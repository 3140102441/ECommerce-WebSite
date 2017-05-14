﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace ECommerce.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (context.Users.Any())
            {
                return;
            }

            var users = new ApplicationUser[]
            {
                new ApplicationUser{ UserName = "ctester", PhoneNumber = "16688886666"},
                new ApplicationUser{ UserName = "stester", PhoneNumber = "17455556666"},
            };

            foreach (var item in users)
            {
                userManager.CreateAsync(item, "123456").Wait();
            }


            var sellers = new Seller[]
            {
                new Seller
                {
                    ApplicationUserID = context.Users.Single(i => i.UserName == "stester").Id,
                    CreditCardNumber = "11112222",
                    Description = "A seller for testing."
                }
            };

            foreach (var item in sellers)
            {
                context.Add(item);
                context.SaveChanges();
            }

            var customers = new Customer[]
            {
                new Customer
                {
                    ApplicationUserID = context.Users.Single(i => i.UserName == "ctester").Id,
                    CreditCardNumber = "12121212"
                }
            };

            foreach (var item in customers)
            {
                context.Add(item);
                context.SaveChanges();
            }

            var commodities = new Commodity[]
            {
                new Commodity
                {
                    Price = 20.5M,
                    Name = "Calculus",
                    Description = "A Calculus book.",
                    Quantity = 12,
                    SellerID = context.Seller.Single(
                        i => i.ApplicationUserID == context.Users.Single(j => j.UserName == "stester").Id).ID,
                    Genre = "BOOK"
                }
            };

            foreach (var item in commodities)
            {
                context.Add(item);
                context.SaveChanges();
            }

            var paths = new ImagePath[]
            {
                new Models.ImagePath
                {
                    CommodityID = context.Commodity.Where(i => i.Name == "Calculus").ToList()[0].ID,
                    ExtendedName = ".jpg"
                },
                new Models.ImagePath
                {
                    CommodityID = context.Commodity.Where(i => i.Name == "Calculus").ToList()[0].ID,
                    ExtendedName = ".jpg"
                }
            };

            foreach (var item in paths)
            {
                context.Add(item);
                context.SaveChanges();
            }

            var imageNames = new string[]
            {
                "test1.jpg",
                "test2.jpg"
            };

            var targetID = context.Commodity.Where(j => j.Name == "Calculus").ToList()[0].ID;

            var pathsFetch = context.Path.Where(
                i => i.CommodityID == targetID)
                .ToList();

            for (int i = 0; i < pathsFetch.LongCount(); i++)
            {
                var path = pathsFetch[i];
                var imageName = imageNames[i];
                {
                    using (var writer = new BinaryWriter(File.Open(path.FullPath(), FileMode.Create)))
                    {
                        var content = File.ReadAllBytes(
                            Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", @"TestImage", imageName));
                        writer.Write(content);
                    }
                }
            }
        }
    }
}
