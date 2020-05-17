package app;

import java.awt.Container;

import javax.tv.xlet.XletContext;
import javax.tv.xlet.XletStateChangeException;

import com.dreamer.x2let.app.AppXlet;
import com.dreamer.x2let.ui.core_game.Animator;
import com.dreamer.x2let.ui.core_game.GraphicsManager;
import component.ResourceManager;

import dreamer.biddle.client.x2let.X2letContext;

/**
 * Xlet Main Class : GameXlet class
 */
public class MainXlet extends AppXlet
{
	private Container rootContainer;

	public String game_server_ip;
	public int game_server_port;
	
	public static MainXlet xlet;

	public void initXlet(XletContext ctx, X2letContext x2letContext) throws XletStateChangeException
	{
		xlet = this;
		rootContainer = getRootContainer();
		rootContainer.setBounds(0, 0, 1920, 1080);
		rootContainer.setLayout(null);
		rootContainer.setVisible(true);

		//context = ctx;

//		String screenLogStr = config.getProperty("screenLog");
//		String logLevel = config.getProperty("applog");
//		String GameServerIP = config.getProperty("game.server.ip");
//
//		Logger loger = Logger.getInstance();
//		if (logLevel != null && Integer.parseInt(logLevel) > 0) {
//			loger.setUseDebugLog(true);
//		}
//
//		if (screenLogStr != null && screenLogStr.equalsIgnoreCase(Reference.TRUE))
//		{
//			loger.setUseScreenLog(true);
//		}
//
//		if (GameServerIP != null){
//			game_server_ip = GameServerIP.substring(0, GameServerIP.indexOf(":"));
//			game_server_port = Integer.parseInt(GameServerIP.substring(GameServerIP.indexOf(":")+1));
//		}
	}

	public void startXlet(XletContext ctx) throws XletStateChangeException {
//		TestURLconn.send_memory("[Othello][memory-test]"+Runtime.getRuntime().freeMemory());
		GraphicsManager.createInstance(rootContainer);
		Animator.createInstance();
		Animator.getInstance().activate();
		
		showMainScene(Reference.SCENE_GAME_CANVAS);
	}

	public void pauseXlet(XletContext ctx)
	{
		System.out.println(">>> MainXlet.pause()");
	}

	public void destroyXlet(XletContext ctx) throws XletStateChangeException {
		if (rootContainer != null) rootContainer.removeAll();

		rootContainer.setVisible(false);
		rootContainer = null;

		ResourceManager.getInstance().dispose();
//		Logger.getInstance().destroy();
		
		if( null != Animator.getInstance() )
			Animator.getInstance().dispose();
		
		if( null != GraphicsManager.getInstance() )
			GraphicsManager.getInstance().dispose();

		if (com.dreamer.x2let.util.Logger.getInstance()!= null) {
			com.dreamer.x2let.util.Logger.getInstance().destroy();
		}
	}
}
/*-> End Of File <------------------------------------------------------------*/
