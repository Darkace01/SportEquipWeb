using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;
using System.Collections.Generic;

[assembly: OwinStartupAttribute(typeof(SportEquipWeb.Startup))]
namespace SportEquipWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesUsersCategory();
        }

        private void createRolesUsersCategory()
        {
        }

    }
}
