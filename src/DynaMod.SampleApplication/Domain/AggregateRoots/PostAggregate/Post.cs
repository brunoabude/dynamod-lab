using DynaMod.SampleApplication.Domain.AggregateRoots.BoardAggregate;
using DynaMod.SampleApplication.Domain.AggregateRoots.CommentAggregate;
using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;

namespace DynaMod.SampleApplication.Domain.AggregateRoots.PostAggregate
{
    public class Post
    {
        public string?       Id { get; private set; }
        public string        Title { get; private set; }
        public string        Text { get; private set; }
        public User          Author { get; private set; }
        public Board         Board { get; private set; }
        public int           Version { get; private set; }
        public List<Comment> Comments { get; private set; }

        public Post(string title, string text, User author, Board board)
        {
            Title    = title;
            Text     = text;
            Author   = author;
            Board    = board;
            Version  = 0;
            Comments = new List<Comment>();
        }

        public void SetBoard(Board board)
        {
            Board = board;
        }

        public void AddComment(string comment_text, User author)
        {
            Comments.Add(new Comment(comment_text, this, author));
        }
    }
}
