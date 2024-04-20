using DynaMod.SampleApplication.Domain.AggregateRoots.PostAggregate;
using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;

namespace DynaMod.SampleApplication.Domain.AggregateRoots.BoardAggregate
{
    public class Board
    {
        public string     Id { get; private set; }
        public string     BoardName { get; private set; }
        public List<Post> Posts { get; private set; }

        public Board(string id, string boardName)
        {
            Id        = id;
            BoardName = boardName;
            Posts     = new List<Post>();
        }

        public Post CreatePost(string title, string text, User author)
        {
            var post = new Post(title, text, author, this);
            Posts.Add(post);
            return post;
        }
    }
}
