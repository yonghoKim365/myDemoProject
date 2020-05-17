package app;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;


public class TestURLconn
{
	public static void send_memory(String str)
	{


        String sLoginServerUrl = null;


//        sLoginServerUrl = "http://211.238.242.232/logview.php?ticket=";
        sLoginServerUrl = "http://211.238.242.232/logview.php?ticket=";

		try
		{
			URL url = null;
			url = new URL(sLoginServerUrl+str);

			URLConnection urlconn = url.openConnection();

			BufferedReader dis = new BufferedReader(new InputStreamReader(urlconn.getInputStream(), "utf-8"));
			dis.close();
			url = null;

		} catch (FileNotFoundException fe) {
			System.out.println("File not found: " + fe);
		} catch (IOException ie) {
			System.out.println("File is corrupted: " + ie);
		} catch (Exception e) {
			e.getStackTrace();
		}

	}
}
