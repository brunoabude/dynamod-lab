using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DynaMod.Generators
{
    internal class LoadMethodBuilder(MethodDeclarationSyntax original_load_method, ClassDeclarationSyntax class_syntax)
    {
        private static Dictionary<string, string> predefined_types_to_json_token_map = new Dictionary<string, string>
        {
            ["int"] = "Int",
            ["double"] = "Double",
            ["string"] = "String",
            ["float"] = "Double",
        };

        private BlockSyntax body = Block();
        private readonly MethodDeclarationSyntax partial_declaration = original_load_method;
        private readonly ClassDeclarationSyntax class_declaration = class_syntax;
        private readonly ParameterSyntax json_reader_arg = original_load_method.DescendantNodes().OfType<ParameterSyntax>().Single();
        private const string instance_variable_name = "instance";

        private IdentifierNameSyntax json_reader_identifier => IdentifierName(json_reader_arg.Identifier);

        public BlockSyntax Build()
        {

            Add(DeclareVariable_New(partial_declaration.ReturnType, instance_variable_name));
            AddExpression(Assert_ObjectStart());
            
            var while_statements = List<StatementSyntax>();
            while_statements = while_statements.Add(ExpressionStatement(Assert(Token_eq("PropertyName"))));

            var switch_statements = List<SwitchSectionSyntax>();
            
            foreach(var property in class_declaration.DescendantNodes().OfType<PropertyDeclarationSyntax>())
            {
                var case_section = SwitchSection();
                case_section = case_section.AddLabels([CaseSwitchLabel(StringLiteral(property.Identifier.Text))]);
                var section_statements = List<StatementSyntax>();

                if (property.Type is PredefinedTypeSyntax ptype)
                {
                    if (predefined_types_to_json_token_map.ContainsKey(ptype.Keyword.Text))
                    {
                        string token_type = predefined_types_to_json_token_map[ptype.Keyword.Text];

                        var read_and_assert = Assert(LogicalAnd(JsonReader_InvokeMethod("Read"), Token_eq(token_type)));

                        var cast_expression = CastExpression(ptype, JsonReader_dot("Value"));

                        var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            AccessMember(IdentifierName(instance_variable_name), property.Identifier.Text),
                            cast_expression);

                        section_statements = section_statements.Add(ExpressionStatement(read_and_assert));
                        section_statements = section_statements.Add(ExpressionStatement(assignment));
                    } else
                    {
                        section_statements = section_statements.Add(ExpressionStatement(ThrowNotImplemented()));
                    }
                } else if (property.Type is GenericNameSyntax gname)
                {
                    var n_args = gname.TypeArgumentList.Arguments.Count();
                    var name = gname.Identifier.Text;

                    if (name == "List" && n_args == 1)
                    {
                        var new_list = ObjectCreationExpression(property.Type, ArgumentList(), null);

                        var create_new_list = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                                   AccessMember(IdentifierName(instance_variable_name), property.Identifier.Text),
                                                                   new_list);

                        var element_loader = GetAssignableExpression(gname.TypeArgumentList.Arguments[0]);
                        
                        var read_and_assert = Assert(LogicalAnd(JsonReader_InvokeMethod("Read"), Token_eq("ArrayStart")));

                        var list_while_statements = List<StatementSyntax>()
                            .Add(ExpressionStatement(InvocationExpression(AccessMember(IdentifierName(instance_variable_name),
                                            property.Identifier.Text,
                                            "Add"), ArgumentList().AddArguments(Argument(element_loader)))))
                            .Add(ExpressionStatement(Assert(JsonReader_InvokeMethod("Read"))));


                        var list_while = WhileStatement(LogicalNotEquals(JsonReader_dot("Token"),
                                                                         JsonToken_dot("ArrayEnd")), Block().WithStatements(list_while_statements));


                        section_statements = section_statements.Add(ExpressionStatement(create_new_list));
                        section_statements = section_statements.Add(ExpressionStatement(read_and_assert));
                        section_statements = section_statements.Add(ExpressionStatement(JsonReader_InvokeMethod("Read")));
                        section_statements = section_statements.Add(list_while);

                    } else if (name == "Dictionary" && n_args == 2)
                    {
                        var new_dictionary = ObjectCreationExpression(property.Type, ArgumentList(), null);

                        var assign_new_dictionary = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                                                   AccessMember(IdentifierName(instance_variable_name), property.Identifier.Text),
                                                                   new_dictionary);

                        var element_loader = GetAssignableExpression(gname.TypeArgumentList.Arguments[1]);

                        var read_and_assert = Assert(LogicalAnd(JsonReader_InvokeMethod("Read"), Token_eq("ObjectStart")));

                        var proeprty_name_get_value = Invoke(AccessMember(json_reader_identifier, "Value", "ToString"));

                        var declare_property_name = DeclareVariable_Simple(IdentifierName("string"), "property_name");

                        var assign_property_name = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName("property_name"), proeprty_name_get_value);



                        var indexer = BracketedArgumentList().AddArguments(Argument(proeprty_name_get_value));
                        
                        var dictionary_index_acessor = ElementAccessExpression(
                            AccessMember(IdentifierName(instance_variable_name), property.Identifier.Text),
                            indexer);

                        var dictionary_element_assign = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, dictionary_index_acessor, element_loader);



                        var list_while_statements = List<StatementSyntax>()
                            .Add(ExpressionStatement(Assert(LogicalAnd(JsonReader_InvokeMethod("Read"), Token_eq("PropertyName")))))
                            .Add(declare_property_name)
                            .Add(ExpressionStatement(assign_property_name));

                        if (gname.TypeArgumentList.Arguments[1] is PredefinedTypeSyntax ptype2)
                        {
                            string token_type = predefined_types_to_json_token_map[ptype2.Keyword.Text];
                            var read_and_assert2 = Assert(LogicalAnd(JsonReader_InvokeMethod("Read"), Token_eq(token_type)));
                            list_while_statements = list_while_statements.Add(ExpressionStatement(read_and_assert2));
                        }

                        list_while_statements = list_while_statements
                            .Add(ExpressionStatement(dictionary_element_assign))
                            .Add(ExpressionStatement(Assert(JsonReader_InvokeMethod("Read"))));


                        var list_while = WhileStatement(LogicalNotEquals(JsonReader_dot("Token"),
                                                                         JsonToken_dot("ObjectEnd")), Block().WithStatements(list_while_statements));


                        section_statements = section_statements.Add(ExpressionStatement(assign_new_dictionary));
                        section_statements = section_statements.Add(ExpressionStatement(read_and_assert));
                        section_statements = section_statements.Add(list_while);
                    } else  
                    {
                        section_statements = section_statements.Add(ExpressionStatement(ThrowNotImplemented()));
                    }
                }
                else if (property.Type is IdentifierNameSyntax iname)
                {
                    string type_name = iname.Identifier.Text;

                    var invoke_expression = Invoke(AccessMember(iname, "Load"), json_reader_identifier);

                    var assignment = AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                    AccessMember(IdentifierName(instance_variable_name), property.Identifier.Text),
                    invoke_expression);

                    section_statements = section_statements.Add(ExpressionStatement(assignment));
                }

                section_statements = section_statements.Add(BreakStatement());
                case_section = case_section.WithStatements(section_statements);
                switch_statements = switch_statements.Add(case_section);
            }

            var switch_statement = SwitchStatement(Invoke(AccessMember(json_reader_identifier, "Value", "ToString")), switch_statements);

            while_statements= while_statements.Add(switch_statement);

            var while_body = Block().WithStatements(while_statements);

            Add(While_Not_Read_ObjectStart(while_body));
            Add(Return(instance_variable_name));
            return this.body;
        }

        private ExpressionSyntax GetAssignableExpression(TypeSyntax type)
        {
            if (type is PredefinedTypeSyntax ptype)
            {
                if (predefined_types_to_json_token_map.ContainsKey(ptype.Keyword.Text))
                {
                    return CastExpression(ptype, JsonReader_dot("Value"));
                }
                else
                {
                    return ThrowNotImplemented();
                }
            }
            else if (type is GenericNameSyntax)
            {
                return ThrowNotImplemented();
            }
            else if (type is IdentifierNameSyntax iname)
            {
                return Invoke(AccessMember(iname, "Load"), json_reader_identifier);
            }

            throw new NotImplementedException();
        }

        private static ThrowExpressionSyntax ThrowNotImplemented()
        {
            return ThrowExpression(ObjectCreationExpression(IdentifierName("System.NotImplementedException"), ArgumentList(), null));
        }

        private static LiteralExpressionSyntax StringLiteral(string literal_value)
        {
            return LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(literal_value));
        }

        private void Add(StatementSyntax statement)
        {
            body = body.AddStatements(statement);
        }

        private void AddExpression(ExpressionSyntax statement)
        {
            body = body.AddStatements(ExpressionStatement(statement));
        }

        public ExpressionSyntax AccessMember(ExpressionSyntax instance, params string[] members)
        {
            ExpressionSyntax memberAccess = instance;

            foreach(var m in members)
            {
                memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, memberAccess, IdentifierName(m));
            }

            return memberAccess;
        }

        public InvocationExpressionSyntax Invoke(ExpressionSyntax instance, params ExpressionSyntax[] arguments)
        {
            var arg_list = ArgumentList().WithArguments(SeparatedList(arguments.Select(a => Argument(a)).ToArray()));
            return InvocationExpression(instance, arg_list);
        }

        public LocalDeclarationStatementSyntax DeclareVariable_New(TypeSyntax variable_type, string variable_name)
        {
            var initializer          = EqualsValueClause(ObjectCreationExpression(variable_type, ArgumentList(), null));
            var declarator           = VariableDeclarator(Identifier(variable_name), null, initializer);
            var variable_declaration = VariableDeclaration(variable_type, SeparatedList([declarator]));
            
            return LocalDeclarationStatement(variable_declaration);
        }

        public LocalDeclarationStatementSyntax DeclareVariable_Simple(TypeSyntax variable_type, string variable_name)
        {
            var declarator = VariableDeclarator(Identifier(variable_name), null, null);
            var variable_declaration = VariableDeclaration(variable_type, SeparatedList([declarator]));

            return LocalDeclarationStatement(variable_declaration);
        }

        public BinaryExpressionSyntax Token_eq(string token_type)
        {
            return LogicalEquals(JsonReader_dot("Token"), JsonToken_dot(token_type));
        }

        public BinaryExpressionSyntax Token_neq(string token_type)
        {
            return LogicalEquals(JsonReader_dot("Token"), JsonToken_dot(token_type));
        }

        public InvocationExpressionSyntax Assert(ExpressionSyntax expression)
        {
            return InvocationExpression(IdentifierName("Assert"), ArgumentList().AddArguments(Argument(expression)));
        }

        public InvocationExpressionSyntax Assert_ObjectStart()
        {
            var token_eq_object_start = LogicalEquals(JsonReader_dot("Token"), JsonToken_dot("ObjectStart"));
            var json_read__and__token_equals_object_start = LogicalAnd(JsonReader_InvokeMethod("Read"), token_eq_object_start);
            return Assert(json_read__and__token_equals_object_start);
        }

        private static BinaryExpressionSyntax LogicalAnd(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.LogicalAndExpression, left, right);
        }

        private static BinaryExpressionSyntax LogicalEquals(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.EqualsExpression, left, right);
        }

        private static BinaryExpressionSyntax LogicalNotEquals(ExpressionSyntax left, ExpressionSyntax right)
        {
            return BinaryExpression(SyntaxKind.NotEqualsExpression, left, right);
        }

        private static MemberAccessExpressionSyntax JsonToken_dot(string token_name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                                                   IdentifierName("JsonToken"),
                                                                                   IdentifierName(token_name));
        }

        private MemberAccessExpressionSyntax JsonReader_dot(string member_name)
        {
            return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                             IdentifierName(json_reader_arg.Identifier),
                                                             IdentifierName(member_name));
        }

        private InvocationExpressionSyntax JsonReader_InvokeMethod(string method_name)
        {
            return InvocationExpression(JsonReader_dot(method_name), ArgumentList());
        }

        public ReturnStatementSyntax Return(string variable_name)
        {
            return ReturnStatement(IdentifierName(variable_name));
        }

        public WhileStatementSyntax While_Not_Read_ObjectStart(StatementSyntax body)
        {
            //json_reader.Read() && json_reader.Token != JsonToken.ObjectEnd

            var read_sucess = JsonReader_InvokeMethod("Read");
            var token_neq_object_end = LogicalNotEquals(JsonReader_dot("Token"), JsonToken_dot("ObjectEnd"));

            return WhileStatement(LogicalAnd(read_sucess, token_neq_object_end), body);
        }
    }
}
