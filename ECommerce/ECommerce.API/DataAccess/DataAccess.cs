using ECommerce.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql;
using NpgsqlTypes;

namespace ECommerce.API.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string dbconnection;
        private readonly string dateformat;
        public DataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
            dbconnection = this.configuration["ConnectionStrings:DB"];
            dateformat = this.configuration["Constants:DateFormat"];
        }

        public Cart GetActiveCartOfUser(int userid)
        {
            var cart = new Cart();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT COUNT(*) From  \"Carts\" WHERE \"UserId\"=" + userid + " AND \"Ordered\"='false';";
                command.CommandText = query;

                int count = Int32.Parse(command.ExecuteScalar().ToString());
             
                if (count == 0)
                {
                    return cart;
                }

                query = "SELECT COUNT(*) FROM  \"Carts\" WHERE \"UserId\"=" + userid + " AND \"Ordered\"='false';";
                command.CommandText = query;

                int cartid = Int32.Parse(command.ExecuteScalar().ToString());


                query = "select * from  \"CartItems\" where \"CartId\"=" + cartid + ";";
                command.CommandText = query;

                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }

                cart.Id = cartid;
                cart.User = GetUser(userid);
                cart.Ordered = false;
                cart.OrderedOn = "";
            }
            return cart;
        }

        public List<Cart> GetAllPreviousCartsOfUser(int userid)
        {
            var carts = new List<Cart>();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT  \"CartId\" FROM  \"Carts\" WHERE \"UserId\"=" + userid + " AND \"Ordered\"='true';";
                command.CommandText = query;
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var cartid = (int)reader["CartId"];
                    carts.Add(GetCart(cartid));
                }
            }
            return carts;
        }

        public Cart GetCart(int cartid)
        {
            var cart = new Cart();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT * FROM  \"CartItems\" WHERE \"CartId\"=" + cartid + ";";
                command.CommandText = query;

                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    CartItem item = new()
                    {
                        Id = (int)reader["CartItemId"],
                        Product = GetProduct((int)reader["ProductId"])
                    };
                    cart.CartItems.Add(item);
                }
                reader.Close();

                query = "SELECT * FROM  \"Carts\" WHERE CartId=" + cartid + ";";
                command.CommandText = query;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cart.Id = cartid;
                    cart.User = GetUser((int)reader["UserId"]);
                    cart.Ordered = bool.Parse((string)reader["Ordered"]);
                    cart.OrderedOn = (string)reader["OrderedOn"];
                }
                reader.Close();
            }
            return cart;
        }

        public Offer GetOffer(int id)
        {
            var offer = new Offer();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"Offers\" WHERE \"OfferId\"=" + id + ";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    offer.Id = (int)r["OfferId"];
                    offer.Title = (string)r["Title"];
                    offer.Discount = (int)r["Discount"];
                }
            }
            return offer;
        }

        public List<PaymentMethod> GetPaymentMethods()
        {
            var result = new List<PaymentMethod>();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"PaymentMethods\";";
                command.CommandText = query;

                connection.Open();

                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PaymentMethod paymentMethod = new()
                    {
                        Id = (int)reader["PaymentMethodId"],
                        Type = (string)reader["Type"],
                        Provider = (string)reader["Provider"],
                        Available = bool.Parse((string)reader["Available"]),
                        Reason = (string)reader["Reason"]
                    };
                    result.Add(paymentMethod);
                }
            }
            return result;
        }

        public Product GetProduct(int id)
        {
            var product = new Product();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM \"Products\" WHERE \"ProductId\" = " + id + ";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    product.Id = (int)reader["ProductId"];
                    product.Title = (string)reader["Title"];
                    product.Description = (string)reader["Description"];
                    product.Price = (double)reader["Price"];
                    product.Quantity = (int)reader["Quantity"];
                    product.ImageName = (string)reader["ImageName"];

                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);
                }
            }
            return product;
        }

        public List<ProductCategory> GetProductCategories()
        {
            var productCategories = new List<ProductCategory>();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };
                string query = "SELECT * FROM \"ProductCategories\";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var category = new ProductCategory()
                    {
                        Id = (int)reader["CategoryId"],
                        Category = (string)reader["Category"],
                        SubCategory = (string)reader["SubCategory"]
                    };
                    productCategories.Add(category);
                }
            }
            return productCategories;
        }

        public ProductCategory GetProductCategory(int id)
        {
            var productCategory = new ProductCategory();

            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"ProductCategories\" WHERE  \"CategoryId\"=" + id + ";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader r = command.ExecuteReader();
                while (r.Read())
                {
                    productCategory.Id = (int)r["CategoryId"];
                    productCategory.Category = (string)r["Category"];
                    productCategory.SubCategory = (string)r["SubCategory"];
                }
            }

            return productCategory;
        }

        public List<Review> GetProductReviews(int productId)
        {
            var reviews = new List<Review>();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"Reviews\" WHERE \"ProductId\"=" + productId + ";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var review = new Review()
                    {
                        Id = (int)reader["ReviewId"],
                        Value = (string)reader["Review"],
                        CreatedAt = (string)reader["CreatedAt"]
                    };

                    var userid = (int)reader["UserId"];
                    review.User = GetUser(userid);

                    var productid = (int)reader["ProductId"];
                    review.Product = GetProduct(productid);

                    reviews.Add(review);
                }
            }
            return reviews;
        }

        public List<Product> GetProducts(string category, string subcategory, int count)
        {
            var products = new List<Product>();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"Products\" WHERE \"CategoryId\"=(SELECT \"CategoryId\" FROM  \"ProductCategories\" WHERE \"Category\"=@c AND \"SubCategory\"=@s) LIMIT "+ count +" ;";
                command.CommandText = query;
                command.Parameters.Add("@c", NpgsqlDbType.Varchar).Value = category;
                command.Parameters.Add("@s", NpgsqlDbType.Varchar).Value = subcategory;

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var product = new Product()
                    {
                        Id = (int)reader["ProductId"],
                        Title = (string)reader["Title"],
                        Description = (string)reader["Description"],
                        Price = (double)reader["Price"],
                        Quantity = (int)reader["Quantity"],
                        ImageName = (string)reader["ImageName"]
                    };

                    var categoryid = (int)reader["CategoryId"];
                    product.ProductCategory = GetProductCategory(categoryid);

                    var offerid = (int)reader["OfferId"];
                    product.Offer = GetOffer(offerid);

                    products.Add(product);
                }
            }
            return products;
        }

        public User GetUser(int id)
        {
            var user = new User();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "SELECT * FROM  \"Users\" WHERE \"UserId\"=" + id + ";";
                command.CommandText = query;

                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreatedAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                }
            }
            return user;
        }

        public bool InsertCartItem(int userId, int productId)
        {
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();
                string query = "SELECT COUNT(*) FROM  \"Carts\" WHERE \"UserId\"=" + userId + " AND \"Ordered\"='false';";
                command.CommandText = query;
                int count = Int32.Parse(command.ExecuteScalar().ToString());

                if (count == 0)
                {
                    query = "INSERT INTO \"Carts\"  (\"UserId\", \"Ordered\", \"OrderedOn\") VALUES (" + userId + ", 'false', '');";
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }

                query = "SELECT \"CartId\" FROM \"Carts\" WHERE \"UserId\"=" + userId + " AND \"Ordered\"='false';";
                command.CommandText = query;
                int cartId = Int32.Parse(command.ExecuteScalar().ToString());



                query = "INSERT INTO \"CartItems\" (\"CartId\", \"ProductId\") VALUES (" + cartId + ", " + productId + ");";
                command.CommandText = query;
                command.ExecuteNonQuery();
                return true;
            }
        }

        public int InsertOrder(Order order)
        {
            int value = 0;

            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = "INSERT INTO  \"Orders\" (UserId, CartId, PaymentId, CreatedAt) values (@uid, @cid, @pid, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@uid", NpgsqlDbType.Integer).Value = order.User.Id;
                command.Parameters.Add("@cid", NpgsqlDbType.Integer).Value = order.Cart.Id;
                command.Parameters.Add("@cat", NpgsqlDbType.Varchar).Value = order.CreatedAt;
                command.Parameters.Add("@pid", NpgsqlDbType.Integer).Value = order.Payment.Id;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "UPDATE  \"Carts\" SET Ordered='true', OrderedOn='" + DateTime.Now.ToString(dateformat) + "' WHERE \"CartId\"=" + order.Cart.Id + ";";
                    command.CommandText = query;
                    command.ExecuteNonQuery();

                    query = "SELECT TOP 1 Id FROM  \"Orders\" ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = Int32.Parse(command.ExecuteScalar().ToString());
                    
                }
                else
                {
                    value = 0;
                }
            }

            return value;
        }

        public int InsertPayment(Payment payment)
        {
            int value = 0;
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                string query = @"INSERT INTO Payments (PaymentMethodId, UserId, TotalAmount, ShippingCharges, AmountReduced, AmountPaid, CreatedAt) 
                                VALUES (@pmid, @uid, @ta, @sc, @ar, @ap, @cat);";

                command.CommandText = query;
                command.Parameters.Add("@pmid", NpgsqlDbType.Integer).Value = payment.PaymentMethod.Id;
                command.Parameters.Add("@uid", NpgsqlDbType.Integer).Value = payment.User.Id;
                command.Parameters.Add("@ta", NpgsqlDbType.Varchar).Value = payment.TotalAmount;
                command.Parameters.Add("@sc", NpgsqlDbType.Varchar).Value = payment.ShipingCharges;
                command.Parameters.Add("@ar", NpgsqlDbType.Varchar).Value = payment.AmountReduced;
                command.Parameters.Add("@ap", NpgsqlDbType.Varchar).Value = payment.AmountPaid;
                command.Parameters.Add("@cat", NpgsqlDbType.Varchar).Value = payment.CreatedAt;

                connection.Open();
                value = command.ExecuteNonQuery();

                if (value > 0)
                {
                    query = "SELECT TOP 1 Id FROM  \"Payments \" ORDER BY Id DESC;";
                    command.CommandText = query;
                    value = Int32.Parse(command.ExecuteScalar().ToString());

                }
                else
                {
                    value = 0;
                }
            }
            return value;
        }

        public void InsertReview(Review review)
        {
            using NpgsqlConnection connection = new(dbconnection);
            NpgsqlCommand command = new()
            {
                Connection = connection
            };

            string query = "INSERT INTO  \"Reviews\" (\"UserId\", \"ProductId\", \"Review\", \"CreatedAt\") VALUES (@uid, @pid, @rv, @cat);";
            command.CommandText = query;
            command.Parameters.Add("@uid", NpgsqlDbType.Integer).Value = review.User.Id;
            command.Parameters.Add("@pid", NpgsqlDbType.Integer).Value = review.Product.Id;
            command.Parameters.Add("@rv", NpgsqlDbType.Varchar).Value = review.Value;
            command.Parameters.Add("@cat", NpgsqlDbType.Varchar).Value = review.CreatedAt;

            connection.Open();
            command.ExecuteNonQuery();
        }

        public bool InsertUser(User user)
        {
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };
                connection.Open();

                string query = "SELECT COUNT(*) FROM  \"Users\" WHERE \"Email\"='" + user.Email + "';";
                command.CommandText = query;
                int count = Int32.Parse(command.ExecuteScalar().ToString());
                if (count > 0)
                {
                    connection.Close();
                    return false;
                }

                query = "INSERT INTO  \"Users\" (\"FirstName\", \"LastName\", \"Address\", \"Mobile\", \"Email\", \"Password\", \"CreatedAt\", \"ModifiedAt\") values (@fn, @ln, @add, @mb, @em, @pwd, @cat, @mat);";

                command.CommandText = query;
                command.Parameters.Add("@fn", NpgsqlDbType.Varchar).Value = user.FirstName;
                command.Parameters.Add("@ln", NpgsqlDbType.Varchar).Value = user.LastName;
                command.Parameters.Add("@add", NpgsqlDbType.Varchar).Value = user.Address;
                command.Parameters.Add("@mb", NpgsqlDbType.Varchar).Value = user.Mobile;
                command.Parameters.Add("@em", NpgsqlDbType.Varchar).Value = user.Email;
                command.Parameters.Add("@pwd", NpgsqlDbType.Varchar).Value = user.Password;
                command.Parameters.Add("@cat", NpgsqlDbType.Varchar).Value = user.CreatedAt;
                command.Parameters.Add("@mat", NpgsqlDbType.Varchar).Value = user.ModifiedAt;

                command.ExecuteNonQuery();
            }
            return true;
        }

        public string IsUserPresent(string email, string password)
        {
            User user = new();
            using (NpgsqlConnection connection = new(dbconnection))
            {
                NpgsqlCommand command = new()
                {
                    Connection = connection
                };

                connection.Open();
                string query = "SELECT COUNT(*) FROM  \"Users\" WHERE \"Email\"='" + email + "' AND \"Password\"='" + password + "';";
                command.CommandText = query;
                int count = Int32.Parse(command.ExecuteScalar().ToString());
                if (count == 0)
                {
                    connection.Close();
                    return "";
                }

                query = "SELECT * FROM  \"Users\" WHERE \"Email\"='" + email + "' AND \"Password\"='" + password + "';";
                command.CommandText = query;

                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    user.Id = (int)reader["UserId"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Email = (string)reader["Email"];
                    user.Address = (string)reader["Address"];
                    user.Mobile = (string)reader["Mobile"];
                    user.Password = (string)reader["Password"];
                    user.CreatedAt = (string)reader["CreatedAt"];
                    user.ModifiedAt = (string)reader["ModifiedAt"];
                }

                string key = "MNU66iBl3T5rh6H52i69";
                string duration = "60";
                var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("address", user.Address),
                    new Claim("mobile", user.Mobile),
                    new Claim("email", user.Email),
                    new Claim("createdAt", user.CreatedAt),
                    new Claim("modifiedAt", user.ModifiedAt)
                };

                var jwtToken = new JwtSecurityToken(
                    issuer: "localhost",
                    audience: "localhost",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Int32.Parse(duration)),
                    signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(jwtToken);
            }
            return "";
        }
    }
}
