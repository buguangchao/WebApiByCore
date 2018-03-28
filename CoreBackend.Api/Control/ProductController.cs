using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CoreBackend.Api.Dto;
using CoreBackend.Api.Service;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CoreBackend.Api.Entity;
using CoreBackend.Api.Repositories;
using AutoMapper;

namespace CoreBackend.Api.Control
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMailService _mailService;
        private readonly MyContext _context;
        private readonly IProductRepository _productRepository;

        public ProductController(ILogger<ProductController> logger, IMailService mailService, MyContext context, IProductRepository productRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _context = context;
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            //return new JsonResult(new List<Product> {
            //    new Product{
            //        Id = 1,
            //        Name ="p1",
            //        Price =12
            //    },
            //    new Product{
            //        Id = 1,
            //        Name ="p2",
            //        Price =55
            //    }
            //});

            //return new JsonResult(ProductService.Current.products);

            //return Ok(ProductService.Current.Products);

            //var products = _productRepository.GetProducts();
            //var result = new List<ProductWithoutMaterialDto>();
            //foreach (var item in products)
            //{
            //    result.Add(new ProductWithoutMaterialDto {
            //        Id = item.Id,
            //        Name = item.Name,
            //        Price = item.Price,
            //        Description = item.Description
            //    });
            //}

            //return Ok(result);

            var products = _productRepository.GetProducts();
            var result = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id}",Name = "GetProduct")]
        public IActionResult GetProductById(int id, bool includeMaterial = false)
        {
            try
            {
                //throw new Exception("手动抛异常！");
                //return new JsonResult(ProductService.Current.products.SingleOrDefault(p=>p.Id == id));

                //var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);

                //if (null == product)
                //{
                //    return NotFound();
                //}
                //_logger.LogInformation($"id={id} is found!");
                //return Ok(product);

                //var product = _productRepository.GetProduct(id, includeMaterial);
                //if (null == product)
                //{
                //    return NotFound();
                //}

                //if (includeMaterial)
                //{
                //    var dto = new ProductDto {
                //        Id = product.Id,
                //        Name = product.Name,
                //        Price = product.Price,
                //        Description = product.Description
                //    };

                //    foreach (var item in product.Materials)
                //    {
                //        dto.Materials.Add(new MaterialDto {
                //            Id = item.Id,
                //            Name = item.Name
                //        });
                //    }

                //    return Ok(dto);
                //}

                //var productDto = new ProductWithoutMaterialDto {
                //    Id = product.Id,
                //    Name = product.Name,
                //    Price = product.Price,
                //    Description = product.Description
                //};
                //return Ok(productDto);

                var product = _productRepository.GetProduct(id, includeMaterial);
                if (null == product)
                {
                    return NotFound();
                }

                if (includeMaterial)
                {
                    var dto = Mapper.Map<ProductDto>(product);

                    return Ok(dto);
                }

                var productDto = Mapper.Map<ProductWithoutMaterialDto>(product);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"find is={id} happen error", ex);
                return StatusCode(500, "查找时出错!");
            }
        }

        [HttpPost]
        public IActionResult POST([FromBody]ProductCreation productCreation)
        {
            if (null == productCreation)
            {
                return BadRequest();
            }

            if (productCreation.Name.Contains("产品"))
            {
                ModelState.AddModelError("name", "产品名称不能包含产品！");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var x = ProductService.Current.Products.Max(m => m.Id);
            //var newProduct = new Product {
            //    Id = ++x,
            //    Name = productCreation.Name,
            //    Price = productCreation.Price,
            //    Description = productCreation.Description
            //};

            //ProductService.Current.Products.Add(newProduct);
            var newProduct = Mapper.Map<Products>(productCreation);
            _productRepository.AddProduct(newProduct);
            bool isOk = _productRepository.Save();
            if (!isOk)
            {
                return StatusCode(500, "保存出错!");
            }

            var re = Mapper.Map<ProductWithoutMaterialDto>(newProduct);

            return CreatedAtRoute("GetProduct", new { id = re.Id }, re);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ProductModification productCreation)
        {
            if (null == productCreation)
            {
                return BadRequest();
            }

            if (productCreation.Name.Contains("产品"))
            {
                ModelState.AddModelError("name", "产品名称不能包含产品！");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
            var product = _productRepository.GetProduct(id, false);
            if (null == product)
            {
                return NotFound();
            }

            //这里我们使用了Mapper.Map的另一个overload的方法，它有两个参数。
            //这个方法会把第一个对象相应的值赋给第二个对象上。这时候product的state就变成了modified了。
            Mapper.Map(productCreation, product);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存出错!");
            }

            //product.Name = productCreation.Name;
            //product.Price = productCreation.Price;
            //product.Description = productCreation.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductModification> productCreation)
        {
            if (null == productCreation)
            {
                return BadRequest();
            }

            //var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
            var product = _productRepository.GetProduct(id, true);
            if (null == product)
            {
                return NotFound();
            }

            //var toPatch = new ProductModification {
            //    Name = product.Name,
            //    Price = product.Price,
            //    Description = product.Description
            //};
            var toPatch = Mapper.Map<ProductModification>(product);

            productCreation.ApplyTo(toPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!string.IsNullOrEmpty(toPatch.Name) && toPatch.Name.Contains("产品"))
            {
                ModelState.AddModelError("name", "产品名称不能包含产品！");
            }
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //product.Name = toPatch.Name;
            //product.Price = toPatch.Price;
            //product.Description = toPatch.Description;
            Mapper.Map(toPatch, product);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存出错!");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //var product = ProductService.Current.Products.SingleOrDefault(p => p.Id == id);
            var product = _productRepository.GetProduct(id, true);
            if (null == product)
            {
                return NotFound();
            }

            //ProductService.Current.Products.Remove(product);
            _productRepository.DeleteProduct(product);
            if (!_productRepository.Save())
            {
                return StatusCode(500, "保存出错!");
            }

            _mailService.Send("","");
            return NoContent();
        }
    }
}