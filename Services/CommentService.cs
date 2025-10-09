using Microsoft.EntityFrameworkCore;
using MovieBox.Models;
using MovieBox.Data;

namespace MovieBox.Services
{
    public class CommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Comment>> GetCommentsByMovieAsync(int movieId)
        {
            return await _context.Comments
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _context.Comments.FindAsync(comment.Id);
            if (existingComment == null)
                return false;

            _context.Entry(existingComment).CurrentValues.SetValues(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return false;

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VoteCommentAsync(int commentId, bool isHelpful)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return false;

            comment.TotalVotes++;
            if (isHelpful)
            {
                comment.HelpfulVotes++;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetMovieAverageRatingAsync(int movieId)
        {
            var average = await _context.Comments
                .Where(c => c.MovieId == movieId)
                .AverageAsync(c => (double?)c.Rating) ?? 0.0;
                
            return Math.Round(average, 1);
        }
    }
}