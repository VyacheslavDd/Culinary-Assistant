using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant_Main.Services.Seed
{
	public interface ISeedService
	{
		Task CreateAdministratorUserAsync();
	}
}
