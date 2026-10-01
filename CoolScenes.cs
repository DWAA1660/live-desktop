using System.Drawing.Drawing2D;

namespace LiveDesktop;

static class CoolScenes
{
    static void Ellipse(Graphics g, Color c, float x, float y, float w, float h)
    { using var b = new SolidBrush(c); g.FillEllipse(b, x, y, w, h); }
    static void Polygon(Graphics g, Color c, params PointF[] p)
    { using var b = new SolidBrush(c); g.FillPolygon(b, p); }
    static void Line(Graphics g, Color c, float width, float x, float y, float xx, float yy)
    { using var p = new Pen(c, width); g.DrawLine(p, x, y, xx, yy); }
    static void Sky(Graphics g, Color top, Color bottom)
    { using var b = new LinearGradientBrush(new Rectangle(0, 0, 1600, 900), top, bottom, 90f); g.FillRectangle(b, 0, 0, 1600, 900); }
    static void Glow(Graphics g, Color c, float x, float y, float radius, int alpha = 65)
    {
        using var path = new GraphicsPath(); path.AddEllipse(x-radius,y-radius,radius*2,radius*2);
        using var b = new PathGradientBrush(path) { CenterColor = Color.FromArgb(alpha,c), SurroundColors = new[] { Color.FromArgb(0,c) } };
        g.FillPath(b,path);
    }
    public static void Draw(Graphics g, int theme, double t)
    {
        switch(theme) {
            case 13: Koi(g,t); break;
            case 14: Grove(g,t); break;
            case 15: Highway(g,t); break;
            case 16: Aurora(g,t); break;
            default: Galaxy(g,t); break;
        }
    }
    static void Koi(Graphics g, double t)
    {
        Sky(g,Color.FromArgb(8,54,68),Color.FromArgb(22,100,99));
        var random = new Random(12);
        for(int i=0;i<35;i++) {
            float x=random.Next(1600),y=random.Next(900);
            float r=35+(float)((t*18+i*17)%160);
            using var p=new Pen(Color.FromArgb((int)(26*(1-(r-35)/160)),159,225,209),1.5f);
            g.DrawEllipse(p,x-r,y-r*.35f,r*2,r*.7f);
        }
        for(int i=0;i<18;i++) {
            float x=i<9?random.Next(10,310):random.Next(1290,1590),y=random.Next(70,870);
            float r=random.Next(25,66);
            Ellipse(g,Color.FromArgb(65,0,30,24),x-r,y-r*.52f+12,r*2,r*1.05f);
            using var b=new SolidBrush(Color.FromArgb(47+i%3*13,134+i%4*8,92));
            g.FillPie(b,x-r,y-r*.5f,r*2,r,20,318);
            Line(g,Color.FromArgb(80,187,221,128),1,x,y,x+r*.8f,y-r*.13f);
            if(i%4==0) Lotus(g,x,y-15,r*.6f);
        }
        for(int i=0;i<6;i++) {
            double a=t*.12+i*1.05;
            float x=800+(float)Math.Cos(a)*(380+i*24),y=455+(float)Math.Sin(a)*(190+i*11);
            var state=g.Save(); g.TranslateTransform(x,y); g.RotateTransform((float)(a*180/Math.PI+90));
            float wag=(float)Math.Sin(t*3.8+i)*9;
            Ellipse(g,Color.FromArgb(45,0,22,25),-28,-53,58,147);
            Polygon(g,Color.FromArgb(220,232,206,166),new(-8,42),new(-29+wag,83),new(23+wag,80),new(9,42));
            Polygon(g,Color.FromArgb(170,235,221,187),new(-13,-10),new(-42,-1),new(-18,18));
            Polygon(g,Color.FromArgb(170,235,221,187),new(13,-10),new(42,-1),new(18,18));
            Ellipse(g,Color.FromArgb(246,238,214),-20,-52,40,110);
            Color patch=i%2==0?Color.FromArgb(226,93,51):Color.FromArgb(234,164,60);
            Ellipse(g,patch,-18,-45,31,31); Ellipse(g,patch,-8,0,26,34);
            if(i%3==0) Ellipse(g,Color.FromArgb(38,48,49),-12,-12,16,25);
            Ellipse(g,Color.FromArgb(23,40,44),-15,-36,4,5); Ellipse(g,Color.FromArgb(23,40,44),11,-36,4,5);
            Line(g,Color.FromArgb(130,255,255,224),2,-5,-43,-9,-18);
            g.Restore(state);
        }
        for(int i=0;i<14;i++) {
            float x=(float)((i*137+t*12)%1600),y=100+i*57+(float)Math.Sin(t*.6+i)*20;
            Ellipse(g,Color.FromArgb(180,255,195,197),x,y,12,6);
        }
    }
    static void Lotus(Graphics g,float x,float y,float s)
    {
        for(int j=0;j<7;j++) {
            var state=g.Save();g.TranslateTransform(x,y);g.RotateTransform(j*51);
            Ellipse(g,Color.FromArgb(243,163+j*5,189+j*5),-s*.22f,-s,s*.44f,s*1.1f);g.Restore(state);
        }
        Ellipse(g,Color.FromArgb(255,219,128),x-s*.18f,y-s*.18f,s*.36f,s*.36f);
    }
    static void Grove(Graphics g,double t)
    {
        Sky(g,Color.FromArgb(8,16,32),Color.FromArgb(19,51,53));
        Glow(g,Color.FromArgb(97,182,190),850,230,450,45);
        Ellipse(g,Color.FromArgb(143,186,185),1180,95,105,105);
        var random=new Random(183);
        for(int i=0;i<23;i++) {
            float x=random.Next(1600),w=random.Next(20,72);
            Polygon(g,Color.FromArgb(30+i%4*3,57+i%3*5,59),new(x,0),new(x+w,0),new(x+w*.85f,900),new(x-w*.3f,900));
            Line(g,Color.FromArgb(35,64,66),w*.27f,x+w*.5f,260,x-100,130);
        }
        for(int layer=0;layer<3;layer++) {
            var p=new List<PointF>{new(0,900)};
            for(int x=0;x<=1600;x+=40) p.Add(new(x,650+layer*75+(float)Math.Sin(x*.006+layer*2)*45));
            p.Add(new(1600,900));Polygon(g,Color.FromArgb(11+layer*3,40-layer*6,39-layer*5),p.ToArray());
        }
        for(int i=0;i<13;i++) {
            float x=80+i*123,y=710+(float)Math.Sin(i*2)*80,s=30+i%4*17;
            Glow(g,Color.FromArgb(98,230,203),x,y-s,130,23);
            using var stem=new Pen(Color.FromArgb(172,210,195),s*.2f){StartCap=LineCap.Round,EndCap=LineCap.Round};
            g.DrawLine(stem,x,y,x+6,y-s);
            using var cap=new GraphicsPath();cap.AddBezier(x-s,y-s,x-s*.8f,y-s*2.15f,x+s*.8f,y-s*2.15f,x+s,y-s);cap.AddBezier(x+s,y-s,x+s*.5f,y-s*.65f,x-s*.5f,y-s*.65f,x-s,y-s);
            using var b=new LinearGradientBrush(new RectangleF(x-s,y-s*2,s*2,s*1.4f),Color.FromArgb(111,218,201),Color.FromArgb(30,95,110),90f);g.FillPath(b,cap);
            for(int j=0;j<4;j++)Ellipse(g,Color.FromArgb(188,222,236,209),x-s*.65f+j*s*.38f,y-s*1.3f-(j%2)*s*.2f,s*.15f,s*.1f);
        }
        for(int i=0;i<65;i++) {
            float x=(float)((i*127.17+t*(8+i%6))%1700)-50+(float)Math.Sin(t*.8+i)*25;
            float y=220+(i*79)%570+(float)Math.Cos(t*.7+i*2)*25;
            float pulse=(float)(.5+.5*Math.Sin(t*2+i));
            Glow(g,Color.FromArgb(217,252,130),x,y,17, (int)(35+60*pulse));
            Ellipse(g,Color.FromArgb((int)(120+135*pulse),235,255,167),x-2,y-2,4,4);
        }
        for(int i=0;i<45;i++) {
            float x=i*37,y=860+(float)Math.Sin(i*1.7)*20;
            Line(g,Color.FromArgb(35,86,71),2,x,y,x-15+(float)Math.Sin(t+i)*4,y-32-i%4*9);
        }
    }
    static void Highway(Graphics g,double t)
    {
        Sky(g,Color.FromArgb(18,8,43),Color.FromArgb(116,26,91));
        Glow(g,Color.FromArgb(255,74,147),800,335,340,65);
        using(var path=new GraphicsPath()) {
            path.AddEllipse(590,125,420,420);var state=g.Save();g.SetClip(path);
            using var b=new LinearGradientBrush(new Rectangle(590,125,420,420),Color.FromArgb(255,211,109),Color.FromArgb(250,50,137),90f);g.FillRectangle(b,590,125,420,420);
            for(int i=0;i<10;i++) {using var stripe=new SolidBrush(Color.FromArgb(111,27,91));g.FillRectangle(stripe,580,350+i*22,440,5+i*1.4f);}
            g.Restore(state);
        }
        Polygon(g,Color.FromArgb(22,14,48),new(0,480),new(110,359),new(235,436),new(390,300),new(545,452),new(760,470),new(900,450),new(1090,318),new(1300,420),new(1480,350),new(1600,450),new(1600,900),new(0,900));
        using(var road=new SolidBrush(Color.FromArgb(16,12,35)))g.FillRectangle(road,0,490,1600,410);
        for(int i=-14;i<=14;i++)Line(g,Color.FromArgb(95,212,43,171),2,800+i*12,490,800+i*160,900);
        for(int i=0;i<20;i++) {
            float q=(float)((i+t*.9)%20/20);float y=490+q*q*460;
            Line(g,Color.FromArgb(80+(int)(q*130),238,40,184),1+q*2,0,y,1600,y);
        }
        Polygon(g,Color.FromArgb(22,18,43),new(745,490),new(855,490),new(1320,900),new(280,900));
        Line(g,Color.FromArgb(41,235,250),4,745,490,280,900);Line(g,Color.FromArgb(41,235,250),4,855,490,1320,900);
        for(int i=0;i<14;i++) {
            float q=(float)((i+t*1.3)%14/14);float q2=Math.Min(1,q+.035f);
            Line(g,Color.FromArgb(180,254,153,217),2+q*8,800,490+q*q*410,800,490+q2*q2*410);
        }
        for(int i=0;i<9;i++) {
            float q=(float)((i+t*.32)%9/9),y=490+q*q*420,w=10+q*80,h=20+q*220;
            foreach(int side in new[]{-1,1}) {
                float x=800+side*(100+q*710);Line(g,Color.FromArgb(41,141,160),4,x,y,x,y-h);
                Line(g,Color.FromArgb(255,86,196),3,x,y-h,x-side*w,y-h);
                Glow(g,Color.FromArgb(238,56,183),x-side*w,y-h,15+q*30,95);
            }
        }
        var r=new Random(51);for(int i=0;i<60;i++)Ellipse(g,Color.FromArgb(100,206,178,252),r.Next(1600),r.Next(230),2,2);
    }
    static void Aurora(Graphics g,double t)
    {
        Sky(g,Color.FromArgb(4,13,32),Color.FromArgb(18,46,62));
        var random=new Random(47);
        for(int i=0;i<150;i++) {
            float x=random.Next(1600),y=random.Next(610);int a=(int)(90+90*(.5+.5*Math.Sin(t*.8+i)));
            Ellipse(g,Color.FromArgb(a,208,232,248),x,y,i%11==0?3:1.5f,i%11==0?3:1.5f);
        }
        for(int band=0;band<3;band++) {
            for(int x=-30;x<1650;x+=9) {
                float top=110+band*73+(float)Math.Sin(x*.003+t*.3+band)*65+(float)Math.Sin(x*.009-t*.2)*30;
                float bottom=top+95+(float)Math.Sin(x*.005+t*.6+band)*65;
                Color color=band==0?Color.FromArgb(79,249,185):band==1?Color.FromArgb(66,205,204):Color.FromArgb(163,100,232);
                using var b=new LinearGradientBrush(new PointF(x,top),new PointF(x,bottom+190),Color.FromArgb(0,color),Color.FromArgb(0,color));
                b.InterpolationColors=new ColorBlend { Colors=new[]{Color.FromArgb(0,color),Color.FromArgb(35,color),Color.FromArgb(95,color),Color.FromArgb(0,color)},Positions=new[]{0f,.32f,.58f,1f} };
                g.FillRectangle(b,x,top,11,bottom+190-top);
            }
        }
        Ellipse(g,Color.FromArgb(210,226,225),1300,85,65,65);
        Mountain(g,Color.FromArgb(32,63,83),570,180,91);
        Mountain(g,Color.FromArgb(43,77,98),650,240,92);
        using(var lake=new LinearGradientBrush(new Rectangle(0,670,1600,230),Color.FromArgb(14,42,60),Color.FromArgb(7,22,38),90f))g.FillRectangle(lake,0,670,1600,230);
        for(int i=0;i<70;i++) {
            float y=690+i*3,x=700+(float)Math.Sin(i*.6+t*.5)*160,w=30+i*5;
            Line(g,Color.FromArgb(14+i%3*8,83,210,183),1,x-w,y,x+w,y);
        }
        for(int i=0;i<17;i++) {
            float x=i<8?i*48:1220+(i-8)*48,y=720+i%3*12,h=85+i%4*38;
            Pine(g,x,y,h,Color.FromArgb(6,24,32));
        }
        Polygon(g,Color.FromArgb(7,22,30),new(0,820),new(200,790),new(330,840),new(560,900),new(0,900));
        Polygon(g,Color.FromArgb(7,22,30),new(1600,800),new(1380,810),new(1200,900),new(1600,900));
    }
    static void Mountain(Graphics g,Color color,float baseline,float height,int seed)
    {
        var r=new Random(seed);var points=new List<PointF>{new(0,700)};
        for(int x=0;x<=1700;x+=150)points.Add(new(x,baseline-r.Next((int)height)));
        points.Add(new(1600,720));Polygon(g,color,points.ToArray());
        for(int i=1;i<points.Count-2;i++) {
            PointF p=points[i]; if(i%2==0)Polygon(g,Color.FromArgb(91,124,145),new(p.X-29,p.Y+35),p,new(p.X+45,p.Y+49),new(p.X+10,p.Y+31),new(p.X-5,p.Y+40));
        }
    }
    static void Pine(Graphics g,float x,float y,float h,Color c)
    {
        Line(g,c,6,x,y,x,y-h);
        for(int i=0;i<5;i++) {float yy=y-h+i*h*.15f,w=h*(.12f+i*.055f);Polygon(g,c,new(x,yy),new(x-w,yy+h*.38f),new(x+w,yy+h*.38f));}
    }
    static void Galaxy(Graphics g,double t)
    {
        Sky(g,Color.FromArgb(3,6,21),Color.FromArgb(17,9,37));
        for(int i=0;i<14;i++) {
            float x=180+i*97+(float)Math.Sin(t*.08+i)*20,y=600-i*24+(float)Math.Cos(t*.12+i)*40;
            Glow(g,i%2==0?Color.FromArgb(76,55,173):Color.FromArgb(32,114,156),x,y,170+i%3*70,40);
        }
        var r=new Random(127);
        for(int i=0;i<250;i++) {
            float x=(float)((r.Next(1600)+t*(2+i%4))%1600),y=r.Next(900),s=i%17==0?3.5f:1.5f;
            int alpha=(int)(100+140*(.5+.5*Math.Sin(t*(.4+i%3*.2)+i)));
            Color c=i%3==0?Color.FromArgb(alpha,139,203,248):Color.FromArgb(alpha,238,229,255);
            Ellipse(g,c,x,y,s,s);
            if(i%31==0) {Glow(g,Color.FromArgb(144,187,255),x,y,18,70);Line(g,c,1,x-5,y+1,x+7,y+1);Line(g,c,1,x+1,y-5,x+1,y+7);}
        }
        float px=1160+(float)Math.Sin(t*.13)*30,py=300+(float)Math.Cos(t*.12)*15;
        Glow(g,Color.FromArgb(120,126,242),px,py,260,45);
        var state=g.Save();g.TranslateTransform(px,py);g.RotateTransform(-24);
        using(var ring=new Pen(Color.FromArgb(85,168,172,228),22))g.DrawEllipse(ring,-230,-52,460,104);
        using(var planet=new GraphicsPath()) {
            planet.AddEllipse(-118,-118,236,236);
            using var b=new PathGradientBrush(planet){CenterPoint=new(-55,-62),CenterColor=Color.FromArgb(175,177,246),SurroundColors=new[]{Color.FromArgb(29,24,75)}};g.FillPath(b,planet);
            var clip=g.Save();g.SetClip(planet);
            for(int j=0;j<9;j++) {using var p=new Pen(Color.FromArgb(30,215,175,232),9+j%3*4);g.DrawBezier(p,-150,-100+j*25,-50,-65+j*25,50,-125+j*25,150,-85+j*25);}
            g.Restore(clip);
        }
        using(var ring=new Pen(Color.FromArgb(125,183,170,228),18))g.DrawArc(ring,-230,-52,460,104,0,180);
        g.Restore(state);
        float moonx=300+(float)Math.Sin(t*.09)*15,moony=710+(float)Math.Cos(t*.13)*13;
        Glow(g,Color.FromArgb(51,156,168),moonx,moony,110,60);
        using(var p=new GraphicsPath()) {p.AddEllipse(moonx-56,moony-56,112,112);using var b=new PathGradientBrush(p){CenterPoint=new(moonx-22,moony-24),CenterColor=Color.FromArgb(102,220,219),SurroundColors=new[]{Color.FromArgb(13,62,96)}};g.FillPath(b,p);}
        for(int i=0;i<2;i++) {
            double cycle=(t+i*7)%16; if(cycle<2.4) {
                float x=100+(float)cycle*700,y=50+i*170+(float)cycle*190;
                using var p=new Pen(Color.FromArgb((int)(190*(1-cycle/2.4)),206,238,255),2);g.DrawLine(p,x,y,x-100,y-27);Glow(g,Color.FromArgb(174,226,255),x,y,15,90);
            }
        }
    }
}
