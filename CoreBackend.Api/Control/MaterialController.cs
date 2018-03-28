using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CoreBackend.Api.Service;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Dto;
using AutoMapper;

namespace CoreBackend.Api.Control
{
    //[Route("api/[controller]")]
    [Route("api/Product")]
    public class MaterialController : Controller
    {
        private readonly IProductRepository _productRepository;
        public MaterialController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("{pid}/Materials")]
        public IActionResult GetMaterialsByPid(int pid)
        {
            //var x = ProductService.Current.Products.SingleOrDefault(p => p.Id == pid);
            //if (null == x)
            //{
            //    return NotFound();
            //}

            //return Ok(x.Materials);

            var isExists = _productRepository.ProductExist(pid);
            if (!isExists)
            {
                return NotFound();
            }

            var x = _productRepository.GetMaterialsForProduct(pid);
            if (null == x || 0 == x.Count())
            {
                return NotFound();
            }

            //var list = x.Select(p => new MaterialDto
            //                    {
            //                        Id = p.Id,
            //                        Name = p.Name
            //                    }).ToList();
            var list = Mapper.Map<IEnumerable<MaterialDto>>(x);

            return Ok(list);
        }

        [HttpGet("{pid}/Materials/{mid}")]
        public IActionResult GetMaterialsByPid(int pid, int mid)
        {
            //var x = ProductService.Current.Products.SingleOrDefault(p => p.Id == pid);
            //if (null == x || null == x.Materials)
            //{
            //    return NotFound();
            //}

            //var m = x.Materials.SingleOrDefault(p => p.Id == mid);
            //if (null == m)
            //{
            //    return NotFound();
            //}

            //return Ok(m);

            var isExists = _productRepository.ProductExist(pid);
            if (!isExists)
            {
                return NotFound();
            }

            var x = _productRepository.GetMaterialForProduct(pid, mid);
            if (null == x)
            {
                return NotFound();
            }

            //var m = new MaterialDto {
            //    Id = x.Id,
            //    Name = x.Name
            //};
            var m = Mapper.Map<MaterialDto>(x);

            return Ok(m);
        }
    }
}