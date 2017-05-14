using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace IdeaMarket.DataModel
{
    /// <summary>
    /// Database       : postgres
    /// Data Source    : 127.0.0.1
    /// Server Version : 9.6.1
    /// </summary>
    public partial class MainDB : LinqToDB.Data.DataConnection
    {
        public ITable<File> Files { get { return this.GetTable<File>(); } }
        public ITable<Idea> Ideas { get { return this.GetTable<Idea>(); } }
        public ITable<IdeaCategory> IdeaCategories { get { return this.GetTable<IdeaCategory>(); } }
        public ITable<IdeaFile> IdeaFiles { get { return this.GetTable<IdeaFile>(); } }
        public ITable<User> Users { get { return this.GetTable<User>(); } }
        public ITable<Vendor> Vendors { get { return this.GetTable<Vendor>(); } }

        public MainDB()
            : base( "MainDb" )
        {
            InitDataContext();
        }

        public MainDB( string configuration )
            : base( configuration )
        {
            InitDataContext();
        }

        partial void InitDataContext();
    }

    [Table( Schema = "dbo", Name = "file" )]
    public partial class File
    {
        [Column( "id" ), PrimaryKey, Identity] public int ID { get; set; } // integer
        [Column( "name" ), NotNull] public string Name { get; set; } // character varying(50)
        [Column( "content" ), NotNull] public byte[] Content { get; set; }
        [Column( "content_type" ), NotNull] public string ContentType { get; set; }

        #region Associations

        /// <summary>
        /// fk__idea_file__file_BackReference
        /// </summary>
        [Association( ThisKey = "ID", OtherKey = "FileId", CanBeNull = true, IsBackReference = true )]
        public IEnumerable<IdeaFile> IdeaFiles { get; set; }

        #endregion
    }

    [Table( Schema = "dbo", Name = "idea" )]
    public partial class Idea
    {
        [Column( "id" ), PrimaryKey, Identity] public int ID { get; set; } // integer
        [Column( "owner_id" ), NotNull] public int OwnerId { get; set; } // integer
        [Column( "name" ), NotNull] public string Name { get; set; } // character varying(50)
        [Column( "short_description" ), NotNull] public string ShortDescription { get; set; } // character varying(1000)
        [Column( "full_description" ), NotNull] public string FullDescription { get; set; } // character varying
        [Column( "rating" ), NotNull] public double Rating { get; set; }
        [Column( "status_id" ), NotNull] public IdeaStatus Status { get; set; }
        [Column( "cost" )] public double? Cost { get; set; }

        #region Associations

        /// <summary>
        /// fk__idea_category__idea_BackReference
        /// </summary>
        [Association( ThisKey = "ID", OtherKey = "IdeaId", CanBeNull = true, IsBackReference = true )]
        public IEnumerable<IdeaCategory> Categories { get; set; }

        /// <summary>
        /// fk__idea_file__idea_BackReference
        /// </summary>
        [Association( ThisKey = "ID", OtherKey = "IdeaId", CanBeNull = true, IsBackReference = true )]
        public IEnumerable<IdeaFile> IdeaFiles { get; set; }

        #endregion
    }

    public enum Category
    {
        [System.ComponentModel.DataAnnotations.Display( Name = "IT" )]
        IT = 1,
        [System.ComponentModel.DataAnnotations.Display( Name = "Медицина" )]
        Medicine = 2,
        [System.ComponentModel.DataAnnotations.Display( Name = "Космос" )]
        Cosmos = 3,
        [System.ComponentModel.DataAnnotations.Display( Name = "Пузожители" )]
        Pozilivers = 4,
        [System.ComponentModel.DataAnnotations.Display( Name = "Алкоголь" )]
        Alcohol = 5,
        [System.ComponentModel.DataAnnotations.Display( Name = "Другое" )]
        Others = 6
    }

    public enum Visibility
    {
        [System.ComponentModel.DataAnnotations.Display( Name = "Все" )]
        All = 1,
        [System.ComponentModel.DataAnnotations.Display( Name = "Некоторые" )]
        Somebody = 2,
        [System.ComponentModel.DataAnnotations.Display( Name = "Никто" )]
        Nobody = 3
    }

    public enum IdeaStatus
    {
        [System.ComponentModel.DataAnnotations.Display( Name = "Черновик")]
        Draft = 1,
        [System.ComponentModel.DataAnnotations.Display( Name = "Опубликована" )]
        Published = 2,
        [System.ComponentModel.DataAnnotations.Display( Name = "Не допущена" )]
        Rejected = 3,
        [System.ComponentModel.DataAnnotations.Display( Name = "Ожидает подтверждения" )]
        WaitsForApproval = 4
    }

    [Table( Schema = "dbo", Name = "idea_category" )]
    public partial class IdeaCategory
    {
        [Column( "idea_id" ), NotNull] public int IdeaId { get; set; } // integer
        [Column( "category_id" ), NotNull] public Category Category { get; set; } // integer
    
        #region Associations
    
        /// <summary>
        /// fk__idea_category__idea
        /// </summary>
        [Association( ThisKey = "IdeaId", OtherKey = "ID", CanBeNull = false, KeyName = "fk__idea_category__idea", BackReferenceName = "IdeaCategories" )]
        public Idea Idea { get; set; }

        #endregion
    }

    [Table( Schema = "dbo", Name = "vendor_category" )]
    public partial class VendorCategory
    {
        [Column( "vendor_id" ), NotNull] public int VendorId { get; set; } // integer
        [Column( "category_id" ), NotNull] public Category Category { get; set; } // integer

        #region Associations
        
        [Association( ThisKey = "VendorId", OtherKey = "ID", CanBeNull = false, KeyName = "fk__vendor_category__vendor" )]
        public Vendor Vendor { get; set; }

        #endregion
    }

    [Table( Schema = "dbo", Name = "idea_file" )]
    public partial class IdeaFile
    {
        [Column( "idea_id" ), NotNull] public int IdeaId { get; set; } // integer
        [Column( "file_id" ), NotNull] public int FileId { get; set; } // integer

        #region Associations

        /// <summary>
        /// fk__idea_file__idea
        /// </summary>
        [Association( ThisKey = "IdeaId", OtherKey = "ID", CanBeNull = false, KeyName = "fk__idea_file__idea", BackReferenceName = "IdeaFiles" )]
        public Idea Idea { get; set; }

        /// <summary>
        /// fk__idea_file__file
        /// </summary>
        [Association( ThisKey = "FileId", OtherKey = "ID", CanBeNull = false, KeyName = "fk__idea_file__file", BackReferenceName = "IdeaFiles" )]
        public File File { get; set; }

        #endregion
    }

    [Table( Schema = "dbo", Name = "user" )]
    public partial class User
    {
        [Column( "id" ), PrimaryKey, Identity] public int ID { get; set; } // integer
        [Column( "email" ), NotNull] public string Email { get; set; } // character varying(50)
        [Column( "login" ), NotNull] public string Login { get; set; } // character varying(50)
        [Column( "password" ), NotNull] public string Password { get; set; } // character varying(50)
        [Column( "visibility_id" )] public Visibility Visibility { get; set; }
    }

    [Table( Schema = "dbo", Name = "vendor" )]
    public partial class Vendor
    {
        [Column( "id" ), PrimaryKey, Identity] public int ID { get; set; } // integer
        [Column( "user_id" ), NotNull] public int UserId { get; set; } // integer
        [Column( "short_description" ), Nullable] public string ShortDescription { get; set; } // character varying(1000)

        #region Associations

        /// <summary>
        /// fk__vendor__user
        /// </summary>
        [Association( ThisKey = "UserId", OtherKey = "ID", CanBeNull = false, KeyName = "fk__vendor__user", BackReferenceName = "Vendors" )]
        public User User { get; set; }

        [Association( ThisKey = "ID", OtherKey = "VendorId", CanBeNull = true, IsBackReference = false )]
        public IEnumerable<VendorCategory> Categories { get; set; }

        #endregion
    }

    public static partial class TableExtensions
    {
        public static File Find( this ITable<File> table, int ID )
        {
            return table.FirstOrDefault( t =>
                 t.ID == ID );
        }

        public static Idea Find( this ITable<Idea> table, int ID )
        {
            return table.FirstOrDefault( t =>
                 t.ID == ID );
        }

        public static User Find( this ITable<User> table, int ID )
        {
            return table.FirstOrDefault( t =>
                 t.ID == ID );
        }

        public static Vendor Find( this ITable<Vendor> table, int ID )
        {
            return table.FirstOrDefault( t =>
                 t.ID == ID );
        }
    }
}
