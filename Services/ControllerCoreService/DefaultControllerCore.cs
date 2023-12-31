﻿using Opuestos_por_el_Vertice.Data;
using Opuestos_por_el_Vertice.Models.Services.ViewModels.Account;
using Opuestos_por_el_Vertice.Models.Services.ViewModels.ViewEnvelopment;
using Opuestos_por_el_Vertice.Models.ViewModels;
using Opuestos_por_el_Vertice.Services.DataTranfer;
using Opuestos_por_el_Vertice.Services.ViewEnvelopmentSystem;

namespace Opuestos_por_el_Vertice.Models.Services.ViewEnvelopmentSystem
{
    // remember always the default web site value is the Home
    public class DefaultControllerCore : IControllerCore
    {
        private readonly IRepository _repository;
        private readonly UserViewModel _user;
        public DefaultControllerCore(IRepository repository, UserViewModel user)
        {
            _repository = repository;
            _user = user;
        }

        public async Task<ViewKindViewModel> GetViewEnvelopment(string controllerInput)
        {
            if (controllerInput == "Home") { await _repository.UnbendDb(); }
            string refinedInput = ControllerCoreUtilities.ControllerFunctionsIdentifier.RefineControllerInput(controllerInput);
            var centralInfo = ControllerCoreUtilities.GetBody(refinedInput);

            /*   only one object and flow path for all functions needed   */
            List<PostViewModel> posts = new();
            centralInfo.Categories.ToList()
                .ForEach(category => { posts.AddRange(DtoConverter.GetAllViewModels(_repository.DetailAll(category))); });

            /*   aside settings   */
            AsideViewModel aside = centralInfo.Aside;
            aside.AsidesList.Add(posts);

            var viewClass = new ViewKindViewModel(
                ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageKind(controllerInput),
                ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageTitle(controllerInput),
                posts, null,
                ControllerCoreUtilities.GetBody(controllerInput).Hero, aside,
                null, centralInfo.Admin, _user
                );

            return viewClass;
        }

        public async Task<ViewKindViewModel> GetViewEnvelopment(string controllerInput, int id, string postCategory)
        {
            string refinedInput = ControllerCoreUtilities.ControllerFunctionsIdentifier.RefineControllerInput(controllerInput);
            var centralInfo = ControllerCoreUtilities.GetBody(refinedInput);

            /*   taking both objects needed, with two flow paths   */
            List<PostViewModel> posts = new(); PostViewModel? post = null;
            if (!postCategory.Equals("") && !id.Equals(0)) { post = DtoConverter.GetViewModel(await _repository.DetailOne(postCategory, id)); }
            string[] categories = centralInfo.Categories;
            categories.ToList()
                .ForEach(category => { posts.AddRange(DtoConverter.GetAllViewModels(_repository.DetailAll(category))); });

            /*   hero and page data settings, all features are uniques according to controller input   */
            HeroViewModel hero = ControllerCoreUtilities.GetBody(controllerInput).Hero;
            string pageTitle = ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageTitle(controllerInput);
            if (controllerInput == "Post")
            {
                hero.ImageSources.Clear(); hero.ImageAltSources.Clear(); hero.Titles.Clear(); hero.SubTitles.Clear();
                hero.ImageSources.Add(post.Image); hero.ImageAltSources.Add(post.ImageAlt);
                hero.Titles.Add(post.Title); hero.SubTitles.Add(post.SubTitle);
                pageTitle = String.Format("{0} - {1}", post.Title, post.Category);
            }

            /*   aside settings   */
            AsideViewModel aside = centralInfo.Aside;
            aside.AsidesList.Add(posts);

            var viewClass = new ViewKindViewModel(
                ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageKind(controllerInput), pageTitle,
                posts, post,
                hero, aside,
                null, centralInfo.Admin, _user
                );

            return viewClass;
        }

        public ViewKindViewModel GetViewEnvelopment(string controllerInput, int page, string thisSearch, string postsCategory)
        {
            string refinedInput = ControllerCoreUtilities.ControllerFunctionsIdentifier.RefineControllerInput(controllerInput);
            var centralInfo = ControllerCoreUtilities.GetBody(refinedInput);

            /*   taking posts needed   */
            List<PostViewModel> posts = new();
            string[] categories = centralInfo.Categories;
            categories.ToList()
                .ForEach(category => { posts.AddRange(DtoConverter.GetAllViewModels(_repository.DetailAll(category))); });

            /*   search settings   */
            SearchViewModel search = centralInfo.Searcher; if(string.IsNullOrEmpty(thisSearch)) { thisSearch = ""; }
            search = ControllerCoreUtilities.SearchFunctions.FillSearcher(thisSearch.Trim().ToLower(), search.Action, posts, page);

            /*   aside settings   */
            AsideViewModel aside = centralInfo.Aside;
            aside.AsidesList.Add(posts); aside.SearchData = search;

            var viewClass = new ViewKindViewModel(
                ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageKind(controllerInput),
                ControllerCoreUtilities.ControllerFunctionsIdentifier.GetPageTitle(controllerInput),
                posts, null,
                centralInfo.Hero, aside,
                search, centralInfo.Admin, _user
                );

            return viewClass;
        }
    }
}
