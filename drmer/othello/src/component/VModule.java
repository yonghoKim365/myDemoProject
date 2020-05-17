package component;

import java.awt.AlphaComposite;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.image.ImageObserver;

import org.dvb.ui.DVBAlphaComposite;
import org.dvb.ui.DVBGraphics;
import org.dvb.ui.UnsupportedDrawingOperationException;

public class VModule 
{
	public VModule()
	{
	}
	
	
//	TODO : 숫자입력
	public int drawDigit(Graphics g, int x, int y, long num, int digit, Image img, int w, int h, int t, int opt, ImageObserver io)
	{
		//opt
		//0->앞, 1->뒤, 2->가운데
		int i,tmp;
		int tmpi;
		boolean chkZero = false;
		int tmpx;
		int Numlen;
		int tx,ty;
		int ret;

		ret = 0;
		if(digit > 1 && num == 0)
		{
			tx = x;
			ty = y;
			if(opt == 0)
			{
				tx = x;
				ty = y;
			}
			else if(opt == 1)
			{
				tx = x - (w+t);
				ty = y;
				tx += t;
			}
			else if(opt == 2)
			{
				tx = x-((w+t)/2);
				ty = y;
			}

			g.drawImage(img,tx,ty,tx+w-1,ty+h,
					0,0,w-1,h,io);
		}

		tmpx = x;
		tmp = 1;
		for(i=0;i<digit-1;i++) tmp=tmp*10;

		if(digit == 1)
		{
			if(opt == 2)
			{
				Numlen = 1;
				tmpx = x - ((Numlen * (w+t)) / 2);
			}
			chkZero = true;
		}

		for(i=0;i<digit;i++)
		{
//			if(!isAliveThread)
//				return 0;

			tmpi = (int)(num/tmp)%10;
			if(tmpi > 0 && !chkZero)
			{
				if(opt == 2)
				{
					Numlen = digit - i;
					tmpx = x - ((Numlen * (w+t)) / 2);
				}
				chkZero = true;
			}
			if(chkZero)
			{
				ret += (w+t);
				if(opt == 1)
				{
					tmpx = x-((digit * (w+t)) - (i*(w+t)));

					g.drawImage(img,tmpx+t,y,(tmpx+t)+w-1,y+h,
							w*tmpi,0,(w*tmpi)+w-1,h,
							io);
				}
				else
				{
					g.drawImage(img,tmpx,y,tmpx+w-1,y+h,
							w*tmpi,0,(w*tmpi)+w-1,h,
							io);
					tmpx += (w+t);
				}
			}
			tmp/=10;
		}
		return ret;
	}
	
	
//	TODO : BOX충돌체크
	public static boolean isClash(int x1, int y1, int width1, int height1, int x2, int y2, int width2, int height2)
	{
		if (
				(((x1 >= x2) && (x1 <= x2 + width2)) || ((x1 <= x2) && (x1 + width1 >= x2))) &&
				(((y1 >= y2) && (y1 <= y2 + height2)) || ((y1 <= y2) && (y1 + height1 >= y2)))
		)
		{
			return true;
		}
		return false;
	}
	

	//	TODO : 텍스트 입력
	public void drawCenterString(Graphics g, String str, int x, int y, int width)
	{
		int size = (width - g.getFontMetrics().stringWidth(str)) / 2;
		g.drawString(str, x + size, y);
	}
//	우측 정렬
	public void drawString(Graphics g, String str, int x, int y)
	{
		int size = g.getFontMetrics().stringWidth(str);
		g.drawString(str, x - size, y);
	}
	//가운데 정렬
	public void drawCenterString(Graphics g, String str, int x, int y)
	{
		int size = g.getFontMetrics().stringWidth(str) / 2;
		g.drawString(str, x - size, y);
	}
	
	

	//	TODO : 알파값입력
	public void setAlpha(Graphics g, int alphaData)
	{
		//100 -> 불투명
		try 
		{
			float fAlpha = alphaData /(float)100;

			if (g instanceof DVBGraphics) {
				((DVBGraphics) g).setDVBComposite(DVBAlphaComposite.getInstance(DVBAlphaComposite.SRC_OVER, fAlpha));
			} else {
				((Graphics2D) g).setComposite(AlphaComposite.getInstance(AlphaComposite.SRC_OVER, fAlpha));
			}
		}
		catch (UnsupportedDrawingOperationException e) 
		{
			e.printStackTrace();
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	}

	public void offAlpha(Graphics g)
	{
		try
		{
			if (g instanceof DVBGraphics) {
				((DVBGraphics) g).setDVBComposite(DVBAlphaComposite.getInstance(DVBAlphaComposite.SRC_OVER, 1.0f));
			} else {
				((Graphics2D) g).setComposite(AlphaComposite.getInstance(AlphaComposite.SRC_OVER, 1.0f));
			}
		}
		catch (UnsupportedDrawingOperationException e)
		{
			//			e.printStackTrace();
		}
		catch(Exception e)
		{

		}
	}
}
