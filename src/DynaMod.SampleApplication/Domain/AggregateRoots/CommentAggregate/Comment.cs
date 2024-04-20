using DynaMod.SampleApplication.Domain.AggregateRoots.PostAggregate;
using DynaMod.SampleApplication.Domain.AggregateRoots.UserAggregate;

namespace DynaMod.SampleApplication.Domain.AggregateRoots.CommentAggregate
{
    public class Comment
    {
        public string Text { get; private set; }
        public Post Post { get; private set; }
        public User Author { get; private set; }

        public Comment(string text, Post post, User author)
        {
            Text   = text;
            Post   = post;
            Author = author;
        }
    }
}