

using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Export;
using CarDealer.DTO.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;



namespace CarDealer
{
    public class StartUp
    {
        internal static string filePath;

        public static void Main(string[] args)
        {
            CarDealerContext dbContext = new CarDealerContext();

            Mapper.Initialize(cfg => cfg.AddProfile(typeof(CarDealerProfile)));


            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database was created!");


            //filePath = Helper.InitializeFilePath("suppliers.xml", filePath);
            // filePath = Helper.InitializeFilePath("parts.xml", filePath);
            // filePath = Helper.InitializeFilePath("cars.xml", filePath);
            // filePath = Helper.InitializeFilePath("customers.xml", filePath);
            // filePath = Helper.InitializeFilePath("sales.xml", filePath);

            // var inputXml = File.ReadAllText(filePath);

            // Console.WriteLine(ImportSuppliers(dbContext, inputXml));
            //  Console.WriteLine(ImportParts(dbContext, inputXml));
            //  Console.WriteLine(ImportCars(dbContext,inputXml));
            // Console.WriteLine(ImportCustomers(dbContext, inputXml));
            // Console.WriteLine(ImportSales(dbContext,inputXml));

            var xmlDocument = GetSalesWithAppliedDiscount(dbContext);


            filePath = Helper.InitializeOutputFilePath("sales-discounts.xml", filePath);

            File.WriteAllText(filePath, xmlDocument);

        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            ImportSuppliersDto[] suppliers = Helper.Deserialize<ImportSuppliersDto[]>(inputXml, "Suppliers");

            var result = suppliers.Select(x => new Supplier
            {
                Name = x.Name,
                IsImporter = x.IsImporter
            })
                .ToArray();

            context.Suppliers.AddRange(result);
            context.SaveChanges();
            return $"Successfully imported {result.Length}";

        }
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var suppliersId = context.Suppliers.Select(x => x.Id).ToHashSet();

            var partsDto = Helper.Deserialize<ImportPartsDto[]>(inputXml, "Parts");

            var result = new List<Part>();

            foreach (var partDto in partsDto)
            {
                if (suppliersId.Contains(partDto.SupplierId))
                {
                    result.Add(new Part
                    {
                        Name = partDto.Name,
                        Price = partDto.Price,
                        Quantity = partDto.Quantity,
                        SupplierId = partDto.SupplierId

                    });
                }
            }
            context.Parts.AddRange(result);
            context.SaveChanges();


            return $"Successfully imported {result.Count}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsDto = Helper.Deserialize<ImportCarsDto[]>(inputXml, "Cars");

            var existedParts = context.Parts.Select(x => x.Id).ToHashSet();

            var result = new List<Car>();

            foreach (var carDto in carsDto)
            {
                var currCar = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TravelledDistanceance

                };
                foreach (var partId in carDto.Parts.Select(x => x.Id).Distinct())
                {
                    if (!existedParts.Contains(partId))
                    {
                        continue;
                    }

                    currCar.PartCars.Add(new PartCar
                    {
                        CarId = currCar.Id,
                        PartId = partId

                    });
                }
                result.Add(currCar);

            }

            context.Cars.AddRange(result);
            context.SaveChanges();


            return $"Successfully imported {result.Count}";

        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var customersDto = Helper.Deserialize<ImportCustomersDto[]>(inputXml, "Customers");

            var result = customersDto
                .Select(x => new Customer
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver

                })
                .ToArray();


            context.Customers.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Length}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var salesDto = Helper.Deserialize<ImportSalesDto[]>(inputXml, "Sales");

            var carsId = context.Cars.Select(x => x.Id).ToArray();
            var customersId = context.Customers.Select(x => x.Id).ToArray();

            var result = new List<Sale>();

            foreach (var sale in salesDto)
            {

                if (!carsId.Contains(sale.CarId))
                {
                    continue;
                }

                result.Add(new Sale
                {
                    Discount = sale.Discount,
                    CustomerId = sale.CustomerId,
                    CarId = sale.CarId
                });
            }

            context.Sales.AddRange(result);
            context.SaveChanges();

            return $"Successfully imported {result.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var carsDto = context.Cars.Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(x => new ExportCarsDto
                {
                    Model = x.Model,
                    Make = x.Make,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            var result = Helper.Serializer<ExportCarsDto[]>(carsDto, "cars");

            return result;

        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var carsDto = context.Cars.Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new ExportModelCarDto
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance

                })
                .ToArray();

            return Helper.Serializer<ExportModelCarDto[]>(carsDto, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliersDto = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new ExportSuppliersDto
                {
                    Name = x.Name,
                    Id = x.Id,
                    Parts = x.Parts.Count

                })
                .ToArray();


            return Helper.Serializer<ExportSuppliersDto[]>(suppliersDto, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            //sort all parts by price(descending). 
            //Sort all cars by traveled distance(descending) 
            //then by the model(ascending). Select top 5 records.

            var carsDto = context.Cars
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .Select(x => new ExportCarWithPartsDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(a => new ExportPartsDto
                    {
                        Name = a.Part.Name,
                        Price = a.Part.Price

                    })
                    .OrderByDescending(a => a.Price)
                    .ToArray()

                })
                .ToArray();

            return Helper.Serializer<ExportCarWithPartsDto[]>(carsDto, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //    var currDto = context.Customers       // This is working local NOT for Judge
            //    .Where(x => x.Sales.Count > 0)
            //    .Select(x => new
            //    {
            //        Name = x.Name,
            //        SalesCount = x.Sales.Count,
            //        Sales = x.Sales.Select(x => new
            //        {
            //            carPrice = x.Car.PartCars.Sum(a => a.Part.Price)

            //        }).ToArray()

            //    })
            //    .ToArray();          


            //    var customersDto=currDto            
            //    .Select(x => new ExportCustomerDto
            //    {
            //        Name = x.Name,
            //        BoughtCars = x.SalesCount,
            //        SpentMoney = x.Sales.Sum(a => a.carPrice)

            //    })
            //    .OrderByDescending(x => x.SpentMoney)
            //    .ToArray();

            var customersDto = context.Customers    //this works in JUDGE, but I have local problem
                .Where(x => x.Sales.Count > 0)
                .Select(x => new ExportCustomerDto
                {

                    Name = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.Sum(a => a.Car.PartCars.Sum(p => p.Part.Price))


                })
                    .OrderByDescending(x => x.SpentMoney)
                    .ToArray();


            return Helper.Serializer<ExportCustomerDto[]>(customersDto, "customers");

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var result = context.Sales
                .Select(x => new ExportSaleWithAllDto
                {
                    Car = new ExportSaleCarDto
                    {
                        Model=x.Car.Model,
                        Make=x.Car.Make,
                        TravelledDistance=x.Car.TravelledDistance
                    },
                    Discount=x.Discount,
                    CustomerName=x.Customer.Name,
                    Price = x.Car.PartCars.Sum(p=>p.Part.Price),
                    PriceWithDiscount= x.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - x.Discount / 100)

                }).ToArray();




            return Helper.Serializer(result, "sales");
        }
    }

}