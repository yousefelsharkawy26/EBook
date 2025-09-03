using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class SeedAllCategoies : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.InsertData(
									table: "Categories",
									columns: new[] { "Id", "CategoryName", "Description", "IsApproved" },
									values: new object[,]
									{
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111111", "Fiction", "Stories based on imagination", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111112", "Non-Fiction", "Informative books about real events", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111113", "Science", "Books about scientific topics", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111114", "Technology", "Books about tech and innovations", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111115", "History", "Books about historical events", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111116", "Biography", "Life stories of famous people", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111117", "Children", "Books for children and kids", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111118", "Fantasy", "Fantasy stories and novels", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111119", "Mystery", "Mystery and detective books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111120", "Romance", "Romantic novels", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111121", "Horror", "Scary and horror stories", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111122", "Self-Help", "Books to improve yourself", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111123", "Philosophy", "Philosophical books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111124", "Poetry", "Poems and poetry collections", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111125", "Art", "Art books and painting guides", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111126", "Travel", "Travel guides and stories", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111127", "Health", "Health and wellness books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111128", "Cooking", "Cookbooks and recipes", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111129", "Religion", "Books about religion and spirituality", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111130", "Comics", "Comic books and graphic novels", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111131", "Thriller", "Thriller novels", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111132", "Adventure", "Adventure stories", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111133", "Classic", "Classic literature", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111134", "Crime", "Crime novels", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111135", "Drama", "Drama and theatrical stories", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111136", "Education", "Educational books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111137", "Law", "Legal books and references", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111138", "Music", "Books about music theory and history", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111139", "Psychology", "Books about psychology", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111140", "Sports", "Books about sports and athletes", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111141", "Politics", "Political science books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111142", "Business", "Business and management books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111143", "Economics", "Economics books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111144", "Environment", "Environment and ecology books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111145", "Photography", "Books about photography", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111146", "Architecture", "Books about architecture", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111147", "Graphic Novels", "Graphic novels and comics", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111148", "Science Fiction", "Sci-Fi books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111149", "Satire", "Satirical books", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111150", "Reference", "Reference books and guides", true },
																				{ "b1d6f8e4-1c2a-4c3d-8e5f-001111111151", "Textbook", "School and university textbooks", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111152", "Engineering", "Engineering books", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111153", "Astronomy", "Books about astronomy", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111154", "Mathematics", "Mathematics books", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111155", "Languages", "Books about languages", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111156", "Anthology", "Anthology collections", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111157", "Diaries", "Personal diaries", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111158", "Memoir", "Memoirs of famous people", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111159", "Classic Literature", "Classic novels", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111160", "Magazines", "Magazine publications", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111161", "Graphic Design", "Books about graphic design", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111162", "Cultural Studies", "Books about culture and society", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111163", "Travel Guides", "Travel guidebooks", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111164", "Computer Science", "Books about programming and computing", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111165", "Networking", "Networking and communication books", true },
																				{ "b1d8f8e4-1c2a-4c3d-8e5f-001111111166", "Robotics", "Books about robotics and automation", true }
									});

		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			string[] categoryIds = new string[]
					{
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111111",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111112",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111113",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111114",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111115",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111116",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111117",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111118",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111119",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111120",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111121",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111122",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111123",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111124",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111125",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111126",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111127",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111128",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111129",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111130",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111131",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111132",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111133",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111134",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111135",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111136",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111137",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111138",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111139",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111140",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111141",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111142",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111143",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111144",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111145",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111146",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111147",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111148",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111149",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111150",
																"b1d6f8e4-1c2a-4c3d-8e5f-001111111151",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111152",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111153",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111154",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111155",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111156",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111157",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111158",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111159",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111160",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111161",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111162",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111163",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111164",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111165",
																"b1d8f8e4-1c2a-4c3d-8e5f-001111111166"
					};

			foreach (var id in categoryIds)
			{
				migrationBuilder.DeleteData(
								table: "Categories",
								keyColumn: "Id",
								keyValue: id);
			}
		}
	}
}

