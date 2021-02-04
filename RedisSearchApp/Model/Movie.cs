using NRediSearch;
using StackExchange.Redis;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RedisSearchApp.Model
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [DataMember(Name = "tl")]
        public string Title { get; set; }

        [DataMember(Name = "pt")]
        public string Plot { get; set; }

        [Required]
        [DataMember(Name = "gn")]
        public string Genre { get; set; }

        [Required]
        [DataMember(Name = "ry")]
        public int ReleaseYear { get; set; }

        [Required]
        [DataMember(Name = "rt")]
        public float Rating { get; set; }
    }

    public class MovieSchema
    {
        public static Task BuildScheme(IDatabase db)
        {
            return Task.Run(() =>
            {
                var key = "idx:movies";
                var exists = db.KeyExists(key);
                var client = new Client(key, db);

                var s = new Schema()
                    .AddTextField("tl", 5.0)
                    .AddTextField("ry", 3.0)
                    .AddTextField("gn", 2.0)
                    .AddSortableNumericField("rt");

                try
                {
                    if (exists) { return; }
                    client
                       .CreateIndex(
                           s
                           , new Client.ConfiguredIndexOptions()
                       );
                }
                catch (Exception ex)
                { }
            });
        }
    }
}