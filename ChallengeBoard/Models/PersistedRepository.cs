﻿using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace ChallengeBoard.Models
{
    public class PersistedRepository : IRepository
    {
        public readonly ChallengeBoardContext Db;

        public PersistedRepository(ChallengeBoardContext db)
        {
            Db = db;    
        }

        public IQueryable<Board> Boards
        {
            get { return (Db.Boards.Include(c => c.Owner)); }
        }

        public IQueryable<Competitor> Competitors
        {
            get { return (Db.Competitors); }
        }

        public IQueryable<Match> Matches
        {
            get { return (Db.Matches.Include(c => c.Winner).Include(c => c.Loser)); }
        }

        public IQueryable<UserProfile> UserProfiles
        {
            get { return (Db.UserProfiles); }
            
        }

        public void Add(Board board)
        {
            Db.Boards.Add(board);
        }

        public void Add(Competitor competitor)
        {
            Db.Competitors.Add(competitor);
        }

        public void Add(Match match)
        {
            Db.Matches.Add(match);
        }

        public void Add(UserProfile profile)
        {
            Db.UserProfiles.Add(profile);
        }

        public void Delete(Board board)
        {
            Db.Boards.Remove(board);
        }

        public void Delete(Competitor competitor)
        {
            Db.Competitors.Remove(competitor);
        }

        public void Delete(Match match)
        {
            Db.Matches.Remove(match);
        }

        public void Dispose()
        {
            Db.Dispose();
        }

        public Board GetBoardById(int id)
        {
            return (Boards.FirstOrDefault(x => x.BoardId == id));
        }

        public Board GetBoardByIdWithCompetitors(int id)
        {
            return (Boards.Include(c => c.Competitors.Select(p => p.Profile)).FirstOrDefault(x => x.BoardId == id));
        }

        public IQueryable<Match> GetUnresolvedMatchesByBoardId(int id)
        {
            return (Matches.Where(x => !x.Resolved.HasValue));
        }

        public Match GetMatchById(int id)
        {
            return (Matches.FirstOrDefault(x => x.MatchId == id));
        }

        public void CommitChanges()
        {
            try
            {
                Db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }
    }
}