using System.Collections.Generic;
using Microsoft.Practices.Prism.Modularity;

namespace MagmaTrader.Prism
{
   /// <summary>
	/// http://geekswithblogs.net/jkurtz/archive/2010/01/26/137638.aspx
   /// Allows our shell to probe multiple directories for module assemblies
   /// </summary>
   public class MultipleDirectoryModuleCatalog : DirectoryModuleCatalog
   {
       private readonly IList<string> m_pathsToProbe;
        
       /// <summary>
       /// Initializes a new instance of the MultipleDirectoryModuleCatalog class.
       /// </summary>
       /// <param name="pathsToProbe">An IList of paths to probe for modules.</param>
       public MultipleDirectoryModuleCatalog(IList<string> pathsToProbe)
       {
           this.m_pathsToProbe = pathsToProbe;     
       }
    
       /// <summary>
       /// Provides multiple-path loading of modules over the default <see cref="DirectoryModuleCatalog.InnerLoad"/> method.
       /// </summary>
       protected override void InnerLoad()
       {
           foreach (string path in this.m_pathsToProbe)
           {
               ModulePath = path;
               base.InnerLoad();
           }
       }
   }
}
