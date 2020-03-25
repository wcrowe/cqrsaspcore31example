﻿using ClassifiedAds.Domain.Entities;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ClassifiedAds.Blazor.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Product>> GetProducts()
        {
            await SetBearerToken();

            var response = await _httpClient.GetAsync("api/products", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAs<List<Product>>();
            return products;
        }

        private async Task SetBearerToken()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            if (accessToken != null)
            {
                _httpClient.UseBearerToken(accessToken);
            }
        }

        public async Task<Product> GetProductById(Guid id)
        {
            await SetBearerToken();

            var response = await _httpClient.GetAsync($"api/products/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAs<Product>();
            return product;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Post, "api/products");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var createdProduct = await response.Content.ReadAs<Product>();
            return createdProduct;
        }

        public async Task<Product> UpdateProduct(Guid id, Product product)
        {
            await SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Put, $"api/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = product.AsJsonContent();

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var updatedProduct = await response.Content.ReadAs<Product>();
            return updatedProduct;
        }

        public async Task DeleteProduct(Guid id)
        {
            await SetBearerToken();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/products/{id}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
        }
    }
}
