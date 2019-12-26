using ChatServer.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.DataAccess
{
    public class ChatContext : DbContext
    {
        public ChatContext() : base("ChatDbConnection")
        {

        }

        public DbSet<Message> Messages { get; set; }
    }
}
