using System.Drawing.Drawing2D;
namespace LiveDesktop;

static class CuteScenes
{
    static Color C(int r,int g,int b)=>Color.FromArgb(r,g,b);
    static void E(Graphics g,Color c,float x,float y,float w,float h){using var b=new SolidBrush(c);g.FillEllipse(b,x,y,w,h);}
    static void P(Graphics g,Color c,params PointF[] points){using var b=new SolidBrush(c);g.FillPolygon(b,points);}
    static void L(Graphics g,Color c,float width,params PointF[] points){using var p=new Pen(c,width){StartCap=LineCap.Round,EndCap=LineCap.Round};g.DrawCurve(p,points);}
    static void Sky(Graphics g,Color a,Color b){using var brush=new LinearGradientBrush(new Rectangle(0,0,1600,900),a,b,90);g.FillRectangle(brush,0,0,1600,900);}
    static float S(double t)=> (float)Math.Sin(t);
    static void Stars(Graphics g,double t,int count,Color color,int seed=38){var r=new Random(seed);for(int i=0;i<count;i++){float x=r.Next(1600),y=r.Next(620),s=2+r.Next(4);E(g,Color.FromArgb(90+(int)(80*(1+S(t*.8+i))),color),x,y,s,s);}}
    static void Star(Graphics g,float x,float y,float s,Color c){P(g,c,new(x,y-s),new(x+s*.3f,y-s*.3f),new(x+s,y),new(x+s*.3f,y+s*.3f),new(x,y+s),new(x-s*.3f,y+s*.3f),new(x-s,y),new(x-s*.3f,y-s*.3f));}
    static void Wave(Graphics g,Color color,float y,float a,double t,float period=120){var p=new List<PointF>{new(-10,910),new(-10,y)};for(int x=0;x<=1610;x+=20)p.Add(new(x,y+S(x/period+t)*a));p.Add(new(1610,910));P(g,color,p.ToArray());}
    static void Eyes(Graphics g,float x,float y,float gap,double t){Color ink=C(47,39,49);if(t%7>6.7){L(g,ink,5,new(x-gap-7,y),new(x-gap,y+3),new(x-gap+7,y));L(g,ink,5,new(x+gap-7,y),new(x+gap,y+3),new(x+gap+7,y));}else{E(g,ink,x-gap-6,y-8,13,19);E(g,ink,x+gap-6,y-8,13,19);E(g,Color.White,x-gap-3,y-6,4,5);E(g,Color.White,x+gap-3,y-6,4,5);}}
    static void Cheeks(Graphics g,float x,float y,float gap){E(g,Color.FromArgb(100,250,139,153),x-gap-10,y,23,12);E(g,Color.FromArgb(100,250,139,153),x+gap-10,y,23,12);}
    public static void Draw(Graphics g,int theme,double t){switch(theme){case 3:Fox(g,t);break;case 4:Cat(g,t);break;case 5:Penguin(g,t);break;case 6:Capybara(g,t);break;case 7:Jellyfish(g,t);break;}}
    static void Tree(Graphics g,float x,float y,float h,Color c){P(g,c,new(x,y-h),new(x-h*.32f,y-h*.45f),new(x-h*.14f,y-h*.45f),new(x-h*.43f,y),new(x+h*.43f,y),new(x+h*.14f,y-h*.45f),new(x+h*.32f,y-h*.45f));}
    static void Fox(Graphics g,double t)
    {
        Sky(g,C(21,33,55),C(64,72,83));Stars(g,t,90,C(254,227,165));E(g,C(250,229,179),1230,92,100,100);
        for(int i=0;i<15;i++)Tree(g,i*120-25,660,240+i%4*60,C(30,55,61));
        Wave(g,C(37,64,65),660,25,0,220);for(int i=0;i<9;i++)Tree(g,i*215-60,730,220+i%3*70,C(24,44,47));Wave(g,C(25,44,43),755,16,0,160);
        for(int i=0;i<7;i++)E(g,Color.FromArgb(6+i*2,255,155,64),890+i*10,610+i*9,390-i*20,190-i*18);
        E(g,C(20,32,35),537,757,320,35);
        var st=g.Save();g.TranslateTransform(700,690+S(t*1.2)*3);
        // Curled fluffy tail sits behind the fox's cream chest and paws.
        var tail=g.Save();g.TranslateTransform(-82,38);g.RotateTransform(-13+S(t)*3);E(g,C(206,100,54),-155,-57,205,104);E(g,C(251,220,175),-151,-48,72,89);g.Restore(tail);
        E(g,C(202,97,52),-66,-65,140,140);E(g,C(248,219,175),-43,-24,87,96);
        P(g,C(216,110,57),new(-81,-101),new(-64,-201),new(-15,-133));P(g,C(216,110,57),new(18,-135),new(63,-203),new(84,-101));
        P(g,C(90,52,48),new(-66,-145),new(-60,-183),new(-35,-144));P(g,C(90,52,48),new(36,-144),new(58,-183),new(70,-145));
        E(g,C(222,115,58),-83,-154,166,123);P(g,C(252,230,194),new(-81,-93),new(-38,-107),new(0,-61),new(40,-107),new(82,-93),new(0,-33));
        Eyes(g,0,-107,32,t);Cheeks(g,0,-91,50);E(g,C(43,39,43),-11,-70,22,16);L(g,C(83,55,48),3,new(0,-57),new(0,-48),new(12,-49));
        E(g,C(86,53,44),-61,54,50,26);E(g,C(86,53,44),17,54,50,26);g.Restore(st);
        L(g,C(108,71,49),18,new(950,770),new(1015,754),new(1090,742));L(g,C(139,86,51),18,new(956,738),new(1020,759),new(1090,777));
        for(int i=0;i<5;i++){float h=100+S(t*4+i)*20;P(g,i%2==0?C(247,137,66):C(255,194,88),new(971+i*16,755),new(977+i*16,715-h*.2f),new(989+i*16+S(t*3+i)*10,755-h),new(1008+i*13,755));}
        for(int i=0;i<18;i++){float y=730-(float)((t*45+i*27)%260);float x=1020+S(i*2+t+y*.01)*35;E(g,Color.FromArgb((int)Math.Clamp((y-460)*.7,0,180),255,186,99),x,y,4,6);}
        for(int i=0;i<14;i++){float x=120+i*102+S(t*.6+i)*22,y=620+S(t*.7+i*3)*60;E(g,Color.FromArgb(25,214,247,145),x-7,y-7,20,20);E(g,C(216,240,153),x,y,4,4);}
    }
    static void Cat(Graphics g,double t)
    {
        Sky(g,C(34,24,71),C(126,94,137));Stars(g,t,100,C(250,229,213),63);
        for(int i=0;i<5;i++){float x=(float)((i*370+t*8)%1950)-200;float y=630+i%2*115;E(g,Color.FromArgb(35,211,181,222),x,y,350,95);E(g,Color.FromArgb(30,211,181,222),x+80,y-45,180,120);}
        float dy=S(t*.7)*9;var st=g.Save();g.TranslateTransform(805,435+dy);g.RotateTransform(S(t*.4)*2);
        E(g,Color.FromArgb(20,255,233,169),-242,-242,484,484);E(g,C(249,219,150),-217,-217,434,434);E(g,C(51,34,81),-132,-277,410,410);
        // The cat naps on the lower curve of the crescent.
        L(g,C(190,160,203),27,new(79,141),new(145,148),new(177,112),new(153,81));
        E(g,C(204,181,216),-53,69,147,108);E(g,C(224,204,230),-84,43,110,100);
        P(g,C(224,204,230),new(-77,72),new(-75,16),new(-40,47));P(g,C(224,204,230),new(-27,47),new(9,24),new(12,81));
        P(g,C(235,160,186),new(-67,48),new(-67,28),new(-51,47));P(g,C(235,160,186),new(-14,49),new(0,36),new(1,61));
        L(g,C(67,47,84),4,new(-66,89),new(-56,94),new(-46,89));L(g,C(67,47,84),4,new(-22,89),new(-12,94),new(-2,89));E(g,C(178,106,149),-38,102,11,8);
        for(int i=0;i<3;i++){L(g,C(146,113,160),2,new(-72,105+i*6),new(-96,101+i*7));L(g,C(146,113,160),2,new(3,105+i*6),new(23,101+i*7));}
        E(g,C(234,217,233),-36,151,47,22);E(g,C(234,217,233),20,152,47,22);g.Restore(st);
        for(int i=0;i<7;i++){float x=350+i*145,y=210+S(i*2.1)*100+S(t*.8+i)*12;Star(g,x,y,8+3*S(t+i),C(253,224,155));using var p=new Pen(Color.FromArgb(45,252,229,195),1);g.DrawLine(p,x,y-14,x,0);}
        float sx=(float)((t*80)%1900)-150;L(g,Color.FromArgb(55,248,220,219),3,new(sx-110,180),new(sx-50,200),new(sx,220));Star(g,sx,220,5,C(255,237,201));
    }
    static void Penguin(Graphics g,double t)
    {
        Sky(g,C(139,193,213),C(234,239,232));E(g,C(255,237,188),1240,97,110,110);
        P(g,C(158,190,206),new(-20,615),new(265,242),new(630,615));P(g,C(230,238,235),new(156,385),new(265,242),new(399,414),new(295,373),new(246,399));
        P(g,C(179,204,214),new(435,610),new(931,192),new(1320,610));P(g,C(244,247,240),new(730,361),new(931,192),new(1110,384),new(967,337),new(919,371),new(859,328));
        Wave(g,C(238,247,246),617,23,0,250);Wave(g,C(210,232,236),783,14,0,240);E(g,C(157,207,222),555,743,480,75);E(g,Color.FromArgb(130,219,244,247),590,753,410,43);
        var st=g.Save();g.TranslateTransform(800+S(t*.55)*115,692+S(t*2)*5);g.RotateTransform(S(t*1.4)*6);
        E(g,C(229,154,73),-67,66,65,26);E(g,C(229,154,73),7,66,65,26);
        var wing=g.Save();g.RotateTransform(-14+S(t*2)*14);E(g,C(39,66,83),-106,-70,51,127);g.Restore(wing);wing=g.Save();g.RotateTransform(14-S(t*2)*14);E(g,C(39,66,83),57,-70,51,127);g.Restore(wing);
        E(g,C(39,66,83),-78,-122,156,209);E(g,C(247,244,222),-57,-95,114,167);E(g,C(39,66,83),-65,-169,130,128);E(g,C(247,244,222),-52,-145,66,86);E(g,C(247,244,222),-13,-145,66,86);Eyes(g,0,-111,29,t);Cheeks(g,0,-93,44);P(g,C(244,177,78),new(-18,-88),new(18,-88),new(0,-69));
        using(var b=new SolidBrush(C(214,94,100)))g.FillRectangle(b,-76,-54,152,26);P(g,C(214,94,100),new(35,-37),new(62,-41),new(79,28),new(51,34));for(int i=0;i<3;i++)L(g,C(249,188,174),3,new(56+i*7,19),new(59+i*7,29));
        E(g,C(214,94,100),-68,-178,136,53);E(g,C(214,94,100),-52,-206,104,67);E(g,C(249,210,190),-16,-229,32,32);g.Restore(st);
        var r=new Random(71);for(int i=0;i<100;i++){float x=(r.Next(1700)+S(t*.5+i)*28)-50,y=(float)((r.Next(1100)+t*(12+i%6*4))%1000)-60,s=2+i%4;E(g,Color.FromArgb(185,255,255,252),x,y,s,s);}
    }
    static void Capybara(Graphics g,double t)
    {
        Sky(g,C(140,180,159),C(231,218,171));E(g,C(255,235,172),1180,96,124,124);
        for(int i=0;i<14;i++){float x=i*133-60;L(g,C(87,135,105),10,new(x,590),new(x+8,350),new(x+2,160));for(int j=0;j<4;j++){float y=230+j*76;L(g,C(114,159,110),3,new(x,y),new(x+20,y));E(g,C(102,149,106),x-68,y-35,67,15);E(g,C(102,149,106),x+10,y-45,64,15);}}
        Wave(g,C(101,147,115),542,30,0,230);E(g,C(99,113,95),235,558,1130,395);E(g,C(184,180,144),275,574,1050,341);E(g,C(110,177,164),324,595,952,300);
        for(int i=0;i<15;i++){float a=i*MathF.PI*2/15;E(g,i%2==0?C(134,143,119):C(163,160,128),770+MathF.Cos(a)*480,745+MathF.Sin(a)*137,93,57);}
        for(int i=0;i<8;i++){float y=640+i*27;L(g,Color.FromArgb(80,232,240,194),2,new(390,y),new(630,y+S(t+i)*4),new(1010,y),new(1200,y-4));}
        var st=g.Save();g.TranslateTransform(800,676+S(t)*5);E(g,C(149,107,74),-111,-20,217,99);E(g,C(176,129,85),-104,-107,207,151);E(g,C(148,107,75),-99,-138,46,54);E(g,C(148,107,75),53,-138,46,54);E(g,C(207,158,115),-88,-126,24,26);E(g,C(207,158,115),64,-126,24,26);E(g,C(187,143,96),-105,-89,212,122);E(g,C(203,158,110),-74,-33,148,73);
        L(g,C(74,54,43),4,new(-61,-48),new(-47,-43),new(-34,-48));L(g,C(74,54,43),4,new(34,-48),new(48,-43),new(61,-48));Cheeks(g,0,-32,74);E(g,C(87,63,44),-33,-8,14,8);E(g,C(87,63,44),18,-8,14,8);L(g,C(101,73,51),3,new(-17,15),new(0,18),new(17,15));
        E(g,C(239,164,54),-31,-149,65,58);E(g,C(255,190,74),-20,-145,41,35);L(g,C(73,126,69),3,new(2,-145),new(5,-162));E(g,C(96,150,72),6,-166,29,12);g.Restore(st);
        // Foreground pool clips the submerged body while leaving the relaxed face visible.
        E(g,Color.FromArgb(170,119,185,169),669,735,262,38);L(g,Color.FromArgb(120,225,241,206),3,new(652,754),new(800,763+S(t)*4),new(947,754));
        for(int i=0;i<7;i++){float x=425+i*119+S(t*.4+i)*24,y=600-(float)((t*12+i*34)%185);L(g,Color.FromArgb(25+(int)(20*(1+S(t+i))),255,251,227),10,new(x,y+70),new(x+S(t+i)*17,y+35),new(x+12,y));}
        for(int i=0;i<5;i++){float x=480+i*151+S(t*.3+i)*12,y=685+i%2*80+S(t+i)*4;E(g,C(243,178,65),x,y,28,21);E(g,C(99,152,78),x+13,y-4,14,6);}
    }
    static void Jellyfish(Graphics g,double t)
    {
        Sky(g,C(7,24,52),C(13,73,84));
        for(int i=0;i<6;i++){float x=i*310-80+S(t*.15+i)*60;P(g,Color.FromArgb(9,130,231,232),new(x,0),new(x+60,0),new(x+310,900),new(x+20,900));}
        var r=new Random(91);for(int i=0;i<75;i++){float x=r.Next(1600)+S(t*.4+i)*8,y=(float)((r.Next(1100)-t*(8+i%5*4)+2200)%1100)-100;using var pen=new Pen(Color.FromArgb(45,157,238,223),1);g.DrawEllipse(pen,x,y,3+i%5*2,3+i%5*2);}
        for(int i=0;i<7;i++){float x=130+i*223,y=355+S(i*2.3)*110+S(t*.8+i)*27,scale=i==3?1.15f:.6f+i%3*.12f;var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);float pulse=1+S(t*1.5+i)*.045f;g.ScaleTransform(pulse,1/pulse);Color color=i%2==0?C(146,190,244):C(230,157,218);
            for(int k=0;k<5;k++)E(g,Color.FromArgb(5+k*3,color),-116+k*6,-77+k*4,232-k*12,178-k*8);
            for(int j=0;j<7;j++){float tx=-71+j*24;var pts=new PointF[9];for(int k=0;k<9;k++)pts[k]=new(tx+S(t*1.7+k*.6+j)*17,k*25+40);L(g,Color.FromArgb(130,color),5,pts);}
            using(var b=new LinearGradientBrush(new Rectangle(-100,-70,200,120),Color.FromArgb(170,color),Color.FromArgb(90,color),90))g.FillPie(b,-100,-80,200,160,180,180);
            E(g,Color.FromArgb(160,color),-100,-15,200,45);for(int j=0;j<5;j++)E(g,Color.FromArgb(130,color),-96+j*39,1,40,25);E(g,Color.FromArgb(65,245,250,255),-52,-63,64,24);Eyes(g,0,-7,27,t+i);Cheeks(g,0,9,43);L(g,C(58,59,99),3,new(-9,13),new(0,18),new(9,13));g.Restore(st);}
        Wave(g,C(8,50,67),854,14,0,130);for(int i=0;i<27;i++){float x=i*63,y=875;L(g,i%2==0?C(20,110,108):C(20,86,102),6,new(x,y+50),new(x+S(t*.9+i)*9,y-25),new(x+S(t*.9+i+.6)*20,y-70-i%4*12));}
    }
}
