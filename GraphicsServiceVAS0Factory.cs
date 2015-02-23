using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsService;
using GraphicsService.Graphism;
using GraphicsService.InterfaceCmdGraphicsServer;
using GraphicsService.ServerSettings;
using GraphismVAS0;
using RenderEngine;
using RenderEngineVIZ;
using RenderEngineVIZ.CommandSenderVIZ;
using RenderEngineVIZ.GraphicEngineConn;

using cvfn;

namespace VAS
{
    class GraphicsServiceVAS0Factory
    {
        static public GraphismVAS2015_0 TheGraphism { get; protected set; }

        internal static IGraphicsService Create( ParamsCreateScreenVIZ vizParams, string sceneName, IComputerVisionManager cvManager )
        {
            IRenderEngine renderEngine = createRenderEngine( vizParams, sceneName );

            // Crear el server ---------------------------------------------------------------
            IUserCommandParser userCommandParser = new UserCommandParser();
            IOutputsSettings outputsSettings = new OutputsSettings();

            GraphismVAS2015_0 theGraphism = new GraphismVAS2015_0(renderEngine, cvManager);
            TheGraphism = theGraphism;

            return new GraphicsServiceBasic(userCommandParser, theGraphism, renderEngine, outputsSettings);
        }

        
        // -----------------------------------------------------------------------------------------------------
        private static IRenderEngine createRenderEngine( ParamsCreateScreenVIZ vizParams, string sceneName )
        {

            ICollection<ServerInfo> lstServers = new List<ServerInfo>();
            lstServers.Add(new ServerInfo()
            {
                Name = "Viz1",
                TCPAddress = vizParams.AnimationAddress,
                TCPPort = vizParams.AnimationPort,
                UDPAddress = vizParams.ValueAddress,
                UDPPort = vizParams.ValuePort,
                DataPoolAddress = vizParams.ValueAddress,
                DataPoolPort = vizParams.ValuePort
            });

            IServerSettingsLoader sc_settings = new ServerSettingsLoaderDirect(lstServers);
            IGraphicEngineConn graphicEngineConn = new GraphicEngineConn(sc_settings);
            ICommandSenderVIZ commandSender;
            commandSender = new CommandSenderVIZ(graphicEngineConn);


            RenderEngineVIZ.SceneManager.ISceneConfig sceneConfigLoader =
                new RenderEngineVIZ.SceneManagerVIZ.SceneConfigLoaderBasicVIZ( sceneName );
            RenderEngineVIZ.SceneManager.ISceneManager sceneManager = 
                new RenderEngineVIZ.SceneManagerVIZ.SceneManagerVIZ(sceneConfigLoader);
    
            return new RenderEngineVizRT_2011(commandSender, sceneManager, graphicEngineConn);

        }

    }
}
