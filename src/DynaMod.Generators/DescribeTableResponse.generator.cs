using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DynaMod.Generators
{
    [Generator]
    public class DescribleTableResponseGenerator : ISourceGenerator
    {
        public const string ContractsNamespace = "DynaMod.Client.Contracts";

        public void Execute(GeneratorExecutionContext context)
        {
            INamespaceSymbol       contracts_namespace = GetNamespace(context.Compilation, ContractsNamespace);
            List<INamedTypeSymbol> classes             = GetAllClasses(contracts_namespace);

            foreach(var cls in classes)
            {
                Generate(context, cls);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

        public static INamespaceSymbol GetNamespace(Compilation compilation, string namespace_name)
        {
            INamespaceSymbol target_namespace = compilation.Assembly.GlobalNamespace;

            string[] namespace_components = namespace_name.Split('.');

            foreach (var component in namespace_components)
            {
                target_namespace = target_namespace.GetNamespaceMembers()
                                                   .FirstOrDefault(n => n.Name == component);

                if (target_namespace == null)
                    return null;
            }

            return target_namespace;
        }

        public static List<INamedTypeSymbol> GetAllClasses(INamespaceSymbol namespace_symbol)
        {
            return namespace_symbol.GetMembers()
                                   .Where(m => m.IsType)
                                   .Cast<INamedTypeSymbol>()
                                   .Where(m => m.TypeKind == TypeKind.Class)
                                   .ToList();
        }

        public static NamespaceDeclarationSyntax GetDeclaringNamespace(ClassDeclarationSyntax class_declaration)
        {
            SyntaxNode node = class_declaration;

            while (node is not NamespaceDeclarationSyntax)
            {
                node = node.Parent;

                if (node is CompilationUnitSyntax || node.Parent is null)
                {
                    return null;
                }
            }

            return (NamespaceDeclarationSyntax)node;
        }

        public static CompilationUnitSyntax Generate(GeneratorExecutionContext context, INamedTypeSymbol class_symbol)
        {
            var properties = class_symbol.GetMembers()
                                         .Where(m => m.Kind == SymbolKind.Property)
                                         .Cast<IPropertySymbol>()
                                         .ToList();

            var class_declaration_syntax_tree = class_symbol.DeclaringSyntaxReferences[0]
                                                            .GetSyntax() as ClassDeclarationSyntax;
            
            if (class_declaration_syntax_tree.Modifiers.Select(m => m.Text)
                                                       .Intersect(["public", "partial"])
                                                       .Count() != 2)
            {
                return null;
            }

            var load_method = class_declaration_syntax_tree.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(m => m.Identifier.Text == "Load" && m.Modifiers.Select(m => m.Text)
                                                                      .Intersect(["public", "static", "partial"])
                                                                      .Count() == 3)
                .FirstOrDefault();

            if (load_method == null)
            {
                return null;
            }

            var root_syntax_tree = class_symbol.DeclaringSyntaxReferences[0].SyntaxTree
                                               .GetRoot() as CompilationUnitSyntax;

            var original_using_directives = root_syntax_tree.DescendantNodes()
                                                             .OfType<UsingDirectiveSyntax>()
                                                             .ToList();

            var compilation_unit = CompilationUnit();

            compilation_unit = compilation_unit.AddUsings(original_using_directives
                .Select(u => UsingDirective(u.StaticKeyword, u.Alias, u.Name))
                .ToArray());

            var source_namespace = GetDeclaringNamespace(class_declaration_syntax_tree);
            var namespace_declaration = NamespaceDeclaration(source_namespace.Name);

            var class_declaration = ClassDeclaration(class_declaration_syntax_tree.Identifier)
                .WithModifiers(class_declaration_syntax_tree.Modifiers)
                ;

            var body = Block();

            var builder = new LoadMethodBuilder(load_method, class_declaration_syntax_tree);
            body = builder.Build();

            var load_method_impl = MethodDeclaration(load_method.ReturnType, load_method.Identifier)
                .WithModifiers(load_method.Modifiers)
                .WithParameterList(ParameterList().AddParameters(Parameter(load_method.ParameterList.Parameters[0].AttributeLists, load_method.ParameterList.Parameters[0].Modifiers, load_method.ParameterList.Parameters[0].Type, load_method.ParameterList.Parameters[0].Identifier, null)))
                .WithBody(body);

            class_declaration = class_declaration.AddMembers(load_method_impl);
            namespace_declaration = namespace_declaration.AddMembers(class_declaration);
            compilation_unit = compilation_unit.AddMembers(namespace_declaration);

            context.AddSource($"{class_declaration_syntax_tree.Identifier.Text}.serializer.g.cs", compilation_unit.NormalizeWhitespace().ToFullString());

            return compilation_unit;
        }
    }
}
