using System.Drawing.Drawing2D;

namespace LiveDesktop;

// Coordinates are local to the same 1600 × 900 canvas as the original scenes.
// Travelling creatures wrap beyond the canvas edges; the small movements have
// independent clocks, so the entire landscape never reverses in unison.
static class LivingScenes
{
    static Color C(int r,int g,int b)=>Color.FromArgb(r,g,b);
    static float S(double x)=>(float)Math.Sin(x);
    static float Cos(double x)=>(float)Math.Cos(x);
    static float Phase(double t,double duration)=>(float)(t/duration-Math.Floor(t/duration));
    static float Travel(double t,float speed,float offset,float span=1920)=>Phase(t+offset/speed,span/speed)*span-160;
    static float Smooth(float x){x=Math.Clamp(x,0,1);return x*x*(3-2*x);}
    static void E(Graphics g,Color c,float x,float y,float w,float h){using var b=new SolidBrush(c);g.FillEllipse(b,x,y,w,h);}
    static void R(Graphics g,Color c,float x,float y,float w,float h){using var b=new SolidBrush(c);g.FillRectangle(b,x,y,w,h);}
    static void P(Graphics g,Color c,params PointF[] pts){using var b=new SolidBrush(c);g.FillPolygon(b,pts);}
    static void L(Graphics g,Color c,float width,params PointF[] pts){using var p=new Pen(c,width){StartCap=LineCap.Round,EndCap=LineCap.Round};g.DrawCurve(p,pts);}
    static void Sky(Graphics g,Color top,Color bottom){using var b=new LinearGradientBrush(new Rectangle(0,0,1600,900),top,bottom,90);g.FillRectangle(b,0,0,1600,900);}
    static void Eye(Graphics g,float x,float y,double t,float size=1)
    {
        if(Phase(t,8.7f)>.974f)L(g,C(47,44,44),2*size,new(x-4*size,y),new(x+4*size,y));
        else{E(g,C(43,43,44),x-4*size,y-6*size,8*size,12*size);E(g,C(255,253,238),x-2*size,y-4*size,2.8f*size,3*size);}
    }
    static void Face(Graphics g,float x,float y,double t,float size=1)
    {
        Eye(g,x-17*size,y,t,size);Eye(g,x+17*size,y,t,size);
        E(g,Color.FromArgb(115,246,152,150),x-31*size,y+9*size,16*size,8*size);
        E(g,Color.FromArgb(115,246,152,150),x+15*size,y+9*size,16*size,8*size);
        E(g,C(94,66,59),x-4*size,y+10*size,8*size,6*size);
        L(g,C(94,66,59),1.6f*size,new(x,y+15*size),new(x,y+21*size),new(x+5*size,y+20*size));
    }
    static void Cloud(Graphics g,float x,float y,float scale)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);
        Color c=Color.FromArgb(175,255,249,224);E(g,c,-90,-14,180,38);E(g,c,-63,-41,77,61);E(g,c,-3,-33,68,54);g.Restore(st);
    }
    static void Hill(Graphics g,Color c,float y,float height,double phase)
    {
        var pts=new List<PointF>{new(-20,920),new(-20,y)};
        for(int x=0;x<=1620;x+=40)pts.Add(new(x,y+S(x*.004+phase)*height));pts.Add(new(1620,920));P(g,c,pts.ToArray());
    }
    static void Flower(Graphics g,float x,float y,float h,Color c,double t,int i)
    {
        float tip=x+S(t*.85+i*.7)*h*.11f;L(g,C(76,132,82),2.5f,new(x,y),new(x+S(t*.7+i)*4,y-h*.5f),new(tip,y-h));
        E(g,C(109,164,94),x-15,y-h*.45f,17,7);E(g,C(109,164,94),x+1,y-h*.6f,18,7);
        for(int j=0;j<5;j++){float a=j*MathF.PI*2/5;E(g,c,tip+Cos(a)*7-6,y-h+S(a)*7-6,12,12);}E(g,C(255,213,100),tip-4,y-h-4,8,8);
    }
    static void Butterfly(Graphics g,float x,float y,float scale,double t,Color c)
    {
        float flap=.2f+.8f*Math.Abs(S(t*8));var st=g.Save();g.TranslateTransform(x,y);g.RotateTransform(S(t*1.9)*11);g.ScaleTransform(scale,scale);
        E(g,Color.FromArgb(70,c),-22*flap,-15,43*flap,35);E(g,c,-23*flap,-18,21*flap,25);E(g,c,2*flap,-18,21*flap,25);
        E(g,c,-18*flap,3,16*flap,17);E(g,c,2*flap,3,16*flap,17);L(g,C(76,70,72),3,new(0,-11),new(0,13));L(g,C(76,70,72),1,new(0,-10),new(-5,-17));L(g,C(76,70,72),1,new(0,-10),new(5,-17));g.Restore(st);
    }
    static void Rabbit(Graphics g,float x,float y,float scale,double t,bool eating=false,bool facingLeft=false)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(facingLeft?-scale:scale,scale);
        float sniff=S(t*3.3)*1.3f,chew=eating?Math.Max(0,S(t*4.2))*3:0;
        E(g,Color.FromArgb(35,33,78,60),-58,32,124,22);E(g,C(255,244,216),-62,-7,28,28);
        E(g,C(242,227,206),-47,-45,105,92);E(g,C(255,247,228),-23,-21,62,64);
        E(g,C(229,212,193),-35,29,45,19);E(g,C(247,234,214),22,30,44,20);
        var head=g.Save();g.TranslateTransform(23,chew+sniff);g.RotateTransform(eating?13+S(t*1.7)*2:0);
        var ear=g.Save();g.RotateTransform(S(t*.62)*4);E(g,C(246,230,210),-32,-119,24,78);E(g,C(233,166,167),-26,-108,12,49);g.Restore(ear);
        ear=g.Save();g.RotateTransform(9+S(t*.77+2)*3);E(g,C(246,230,210),7,-121,23,76);E(g,C(233,166,167),13,-110,11,50);g.Restore(ear);
        E(g,C(251,237,217),-41,-62,86,72);Face(g,0,-30,t,.75f);g.Restore(head);
        if(eating){L(g,C(106,160,87),3,new(47,-6),new(69,-12),new(82,-26));E(g,C(152,184,100),65,-25,25,10);}g.Restore(st);
    }
    static void Squirrel(Graphics g,float x,float y,float scale,double t,bool left=false)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(left?-scale:scale,scale);
        var tail=g.Save();g.TranslateTransform(-43,-9);g.RotateTransform(-8+S(t*1.5)*7);E(g,C(153,79,48),-53,-107,69,120);E(g,C(188,103,58),-48,-111,56,92);E(g,C(221,141,75),-36,-95,31,63);g.Restore(tail);
        E(g,C(187,105,62),-27,-36,58,69);E(g,C(248,212,161),-9,-23,29,49);E(g,C(179,94,54),-18,24,48,16);
        P(g,C(180,94,54),new(-3,-63),new(-8,-90),new(13,-73));P(g,C(180,94,54),new(25,-72),new(39,-90),new(43,-55));
        E(g,C(205,122,71),-7,-76,59,49);E(g,C(246,216,174),24,-48,37,21);Eye(g,35,-59,t,.78f);E(g,C(77,52,42),53,-45,9,8);
        E(g,C(115,76,48),17,-13,20,24);E(g,C(89,66,43),13,-17,28,11);L(g,C(190,110,62),10,new(1,-12),new(20,-7));g.Restore(st);
    }
    public static void Draw(Graphics g,int theme,double t)
    {
        switch(theme){case 18:Woodland(g,t);break;case 19:River(g,t);break;case 20:Meadow(g,t);break;}
    }
    static void Woodland(Graphics g,double t)
    {
        Sky(g,C(158,204,187),C(250,233,178));E(g,Color.FromArgb(185,255,234,169),1244,87,142,142);
        for(int i=0;i<5;i++)Cloud(g,Travel(t,6+i,190+i*391),116+i%3*72,.68f+i*.06f);
        Hill(g,C(138,174,121),535,38,.7);Hill(g,C(119,164,113),620,32,2);
        for(int i=0;i<8;i++){float x=i*237-38;R(g,C(118,126,80),x,285,27,338);E(g,C(117,157,107),x-106,198,242,262);E(g,C(146,181,119),x-61,190,170,211);}
        Hill(g,C(133,178,118),706,14,.2);Hill(g,C(172,195,128),826,19,2);
        // Large framing trees leave the picnic clearing and sky open.
        L(g,C(100,77,55),61,new(75,890),new(64,460),new(102,127));L(g,C(100,77,55),25,new(83,371),new(179,245),new(325,181));
        L(g,C(113,84,55),55,new(1510,901),new(1472,476),new(1501,107));L(g,C(113,84,55),27,new(1487,371),new(1360,292),new(1271,240));
        for(int i=0;i<6;i++){E(g,i%2==0?C(73,120,82):C(95,140,89),-173+i*115,-74+i%2*46,288,254);E(g,i%2==0?C(88,132,87):C(117,157,95),1190+i*96,-96+i%2*83,277,241);}
        // The high squirrel makes a circuit with a short pause at the branch tip.
        float sp=Phase(t+6,44),sx=sp<.44f?250+Smooth(sp/.44f)*895:sp<.55f?1145:1145-Smooth((sp-.55f)/.45f)*895;
        Squirrel(g,sx,384-Math.Abs(S(t*3.4))*7,.6f,t,sp>=.55f);
        L(g,C(114,82,54),13,new(198,420),new(684,389),new(1263,399));
        // Gingham picnic blanket, basket, fruit and cups.
        P(g,C(244,220,167),new(572,719),new(1080,725),new(1163,864),new(475,860));
        var clip=g.Save();using(var blanket=new GraphicsPath()){blanket.AddPolygon(new PointF[]{new(572,719),new(1080,725),new(1163,864),new(475,860)});g.SetClip(blanket);for(int i=0;i<12;i++)R(g,Color.FromArgb(55,190,99,105),500+i*58,710,24,160);for(int i=0;i<5;i++)R(g,Color.FromArgb(55,190,99,105),475,724+i*33,700,13);}g.Restore(clip);
        E(g,C(179,131,70),900,757,134,76);L(g,C(145,104,56),8,new(923,764),new(933,709),new(1007,710),new(1021,762));for(int i=0;i<4;i++)L(g,C(209,164,96),3,new(910,774+i*13),new(1025,773+i*13));
        E(g,C(251,240,210),770,817,119,22);E(g,C(225,94,81),787,799,30,26);E(g,C(243,159,66),829,799,27,27);E(g,C(159,181,91),845,787,20,13);
        R(g,C(146,181,171),1050,785,28,33);L(g,C(146,181,171),5,new(1077,789),new(1093,799),new(1078,809));
        // Raccoon breathes, blinks and periodically lifts a berry to its muzzle.
        var rac=g.Save();g.TranslateTransform(688,733);float breathe=1+S(t*1.7)*.015f;g.ScaleTransform(1,breathe);
        var tail=g.Save();g.TranslateTransform(-69,23);g.RotateTransform(-23+S(t*.7)*4);E(g,C(109,108,99),-101,-20,124,47);for(int i=0;i<4;i++)E(g,C(64,70,66),-94+i*27,-17,12,42);g.Restore(tail);
        E(g,C(120,127,115),-67,-76,139,142);E(g,C(211,215,186),-38,-43,81,100);E(g,C(71,80,70),-60,50,48,23);E(g,C(71,80,70),20,51,48,23);
        E(g,C(94,107,99),-70,-164,44,50);E(g,C(94,107,99),29,-164,44,50);E(g,C(222,201,171),-58,-155,23,26);E(g,C(222,201,171),39,-155,23,26);
        E(g,C(167,174,151),-78,-150,158,108);E(g,C(229,228,199),-63,-129,128,81);E(g,C(63,75,69),-66,-118,59,38);E(g,C(63,75,69),8,-118,59,38);
        Eye(g,-31,-101,t,1.15f);Eye(g,34,-101,t,1.15f);E(g,C(233,228,200),-37,-80,79,37);E(g,C(52,61,54),-12,-82,28,19);L(g,C(88,92,74),2,new(1,-66),new(1,-56),new(12,-55));
        float lift=Smooth(Math.Clamp((Phase(t+2,14)-.65f)*8,0,1))*(1-Smooth(Math.Clamp((Phase(t+2,14)-.88f)*9,0,1)));
        L(g,C(117,127,112),20,new(54,-21),new(50,-10-lift*52),new(22,-12-lift*50));E(g,C(192,72,83),12,-30-lift*51,21,22);E(g,C(102,157,85),23,-35-lift*51,15,8);g.Restore(rac);
        Rabbit(g,445,750,.94f,t+3,true,true);Rabbit(g,1155,781,.64f,t+11,true);
        float hop=Phase(t+4,36),rx=hop<.34f?120+Smooth(hop/.34f)*237:hop<.68f?357:357-Smooth((hop-.68f)/.32f)*237;
        Rabbit(g,rx,816-(hop<.34f||hop>.68f?Math.Abs(S(t*3.1))*17:0),.54f,t+8,hop>=.34f&&hop<=.68f,hop>.68f);
        Squirrel(g,1296+S(t*.17)*46,738,.9f,t+4,true);
        for(int i=0;i<23;i++)Flower(g,42+i*71,887-i%3*16,33+i%4*12,i%2==0?C(245,213,143):C(239,172,168),t,i);
        for(int i=0;i<3;i++){float x=Travel(t,16+i*7,180+i*623),y=431+S(t*.39+i)*102+S(t*1.07+i)*13;Butterfly(g,x,y,.64f,t+i*3,i%2==0?C(242,176,91):C(198,145,192));}
        for(int i=0;i<10;i++){float x=190+i*137+S(t*.21+i)*28,y=578+S(t*.32+i*4)*65;E(g,Color.FromArgb(20+(int)(15*(1+S(t*1.1+i))),255,241,145),x-8,y-8,20,20);E(g,Color.FromArgb(100+(int)(65*(1+S(t*1.1+i))),255,241,145),x,y,3,3);}
    }
    static void Otter(Graphics g,float x,float y,float scale,double t,bool diving)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);
        float p=Phase(t,26),dive=diving?Smooth(Math.Clamp((p-.68f)*9,0,1))*(1-Smooth(Math.Clamp((p-.88f)*9,0,1))):0;
        // Clip under the waterline during dives; each family member dives alone.
        g.SetClip(new RectangleF(-160,-160,360,172),CombineMode.Intersect);g.TranslateTransform(0,dive*150);
        g.RotateTransform(-8+dive*18+S(t*.8)*2);L(g,C(125,81,57),25,new(-63,-5),new(-117,-1),new(-136,-12+S(t*1.5)*9));
        E(g,C(143,98,65),-68,-46,142,61);E(g,C(216,173,120),-22,-41,88,43);
        E(g,C(132,89,60),51,-76,21,23);E(g,C(132,89,60),99,-77,21,23);E(g,C(178,126,81),47,-75,81,66);E(g,C(244,210,159),72,-52,62,38);
        Eye(g,77,-49,t,.85f);Eye(g,109,-46,t,.85f);E(g,C(71,51,46),102,-35,13,9);L(g,C(106,75,54),1.5f,new(107,-27),new(109,-20),new(117,-21));
        for(int i=0;i<3;i++)L(g,C(108,79,58),1,new(120,-28+i*4),new(143,-33+i*6));
        L(g,C(117,77,53),12,new(-23,-20),new(-10,-6+S(t*3)*5),new(5,-12));g.Restore(st);
        // Ripples are at the surface beneath the face, never over the animal.
        using var pen=new Pen(Color.FromArgb((int)(120*(1-dive*.6)),235,250,219),2);g.DrawArc(pen,x-112*scale,y+15*scale,228*scale,24*scale,5,162);
        if(dive>.3f){for(int i=0;i<3;i++){float yy=y-(Phase(t*2+i*.23,1)*27),xx=x+55*scale+i*13;using var b=new Pen(Color.FromArgb(100,231,253,235),2);g.DrawEllipse(b,xx,yy,5+i*2,5+i*2);}}
    }
    static void Bird(Graphics g,float x,float y,float scale,double t,bool flying,Color c)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);E(g,c,-17,-15,39,26);E(g,c,10,-27,26,25);P(g,C(231,168,78),new(34,-19),new(48,-14),new(34,-10));Eye(g,28,-19,t,.43f);
        if(flying){L(g,c,12,new(-4,-7),new(-19,-22-S(t*9)*15),new(-33,-25-S(t*9)*27));}else{E(g,Color.FromArgb(110,80,104,126),-10,-8,24,13);L(g,C(116,87,59),2,new(0,9),new(-2,19));L(g,C(116,87,59),2,new(11,9),new(13,19));}g.Restore(st);
    }
    static void River(Graphics g,double t)
    {
        Sky(g,C(158,208,218),C(246,236,186));E(g,C(255,239,185),1220,79,134,134);
        for(int i=0;i<5;i++)Cloud(g,Travel(t,8+i*2,i*419+300),123+i%2*89,.8f+i*.04f);
        Hill(g,C(139,180,149),419,56,1);Hill(g,C(102,156,122),491,38,3);
        for(int i=0;i<13;i++){float x=i*149-40;L(g,C(115,120,84),10,new(x,493),new(x+12,284));E(g,C(90,147,111),x-54,252,141,172);E(g,C(130,175,122),x-38,226,116,143);}
        Hill(g,C(161,190,122),535,29,1);R(g,C(79,172,181),0,544,1600,356);
        using(var br=new LinearGradientBrush(new Rectangle(0,544,1600,356),C(105,190,189),C(48,132,163),90))g.FillRectangle(br,0,544,1600,356);
        // Transparent fish pass under the surface in continuously flowing lanes.
        for(int i=0;i<8;i++){float x=Travel(t,20+i*4,160+i*227),y=680+i%3*64+S(t*.8+i)*6;var st=g.Save();g.TranslateTransform(x,y);P(g,Color.FromArgb(85,243,197,108),new(-22,0),new(-41,-12),new(-41,12));E(g,Color.FromArgb(85,243,197,108),-27,-11,55,22);E(g,Color.FromArgb(90,39,90,99),18,-3,4,4);g.Restore(st);}
        for(int i=0;i<45;i++){float x=Travel(t,13+i%5*7,i*177),y=565+i%12*27;L(g,Color.FromArgb(40+i%3*12,232,253,215),2,new(x,y),new(x+28,y+S(t*.7+i)*2),new(x+51+i%3*14,y));}
        // Two adults and two pups drift at different speeds and depth levels.
        Otter(g,Travel(t,27,1520,1980),628,.84f,t+7,true);
        Otter(g,Travel(t,33,672,2020),732,1.18f,t,false);
        Otter(g,Travel(t,35,421,2020),787,.69f,t+11,true);
        Otter(g,Travel(t,31,150,2020),681,.65f,t+19,true);
        // A leaf boat drifts downstream; rings expand gently where it meets water.
        float lx=Travel(t,19,1221);var leaf=g.Save();g.TranslateTransform(lx,823+S(t*.8)*3);g.RotateTransform(7+S(t*.5)*4);E(g,C(214,194,107),-35,-9,70,18);L(g,C(144,153,87),1,new(-32,0),new(30,0));g.Restore(leaf);
        Hill(g,C(103,151,93),858,25,2);E(g,C(179,182,148),-40,789,253,100);E(g,C(156,167,140),1388,824,254,113);E(g,C(199,205,168),1420,824,88,27);
        L(g,C(127,93,61),16,new(91,815),new(130,720),new(98,639));L(g,C(127,93,61),8,new(123,736),new(194,717));
        Bird(g,182,701,1.02f,t,false,C(94,151,183));Bird(g,105,629,.83f,t+3,false,C(210,143,91));
        for(int i=0;i<3;i++)Bird(g,Travel(t,48+i*8,160+i*555,1980),290+i*64+S(t*.54+i)*15,.61f,t+i,true,C(75,110,129));
        for(int i=0;i<19;i++){float x=i<9?24+i*17:1400+(i-9)*22,h=70+i%4*18;L(g,C(76,132,86),4,new(x,901),new(x+S(t*.68+i)*9,900-h));if(i%3==0)E(g,C(122,87,58),x+S(t*.68+i)*9-5,870-h,11,36);}
        Butterfly(g,1260+S(t*.32)*62,602+S(t*.57)*36,.65f,t+3,C(249,208,115));
    }
    static void Deer(Graphics g,float x,float y,float scale,double t)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);
        float p=Phase(t+1,21),graze=Smooth(Math.Clamp((p-.18f)*7,0,1))*(1-Smooth(Math.Clamp((p-.72f)*7,0,1)));
        P(g,C(186,117,69),new(-91,-52),new(-120,-87-S(t)*5),new(-128,-53),new(-97,-40));
        for(int i=0;i<4;i++){float xx=-60+i*34;L(g,i%2==0?C(162,98,60):C(189,124,75),11,new(xx,-18),new(xx+(i%2==0?-4:5),89));E(g,C(95,69,55),xx-9,85,21,12);}
        E(g,C(196,133,81),-91,-79,166,86);E(g,C(226,185,120),-61,-23,106,26);
        for(int i=0;i<7;i++)E(g,C(252,222,172),-67+i*17,-57+i%2*16,8,6);
        var neck=g.Save();g.TranslateTransform(57,-59);g.RotateTransform(graze*130);
        L(g,C(200,140,86),42,new(0,6),new(15,-71));E(g,C(230,191,132),-4,-90,25,72);
        var ears=g.Save();g.TranslateTransform(23,-119);g.RotateTransform(S(t*1.9)*3);E(g,C(192,129,77),-50,-35,39,19);E(g,C(238,190,136),-45,-31,27,9);E(g,C(192,129,77),19,-44,35,24);E(g,C(238,190,136),23,-37,23,11);g.Restore(ears);
        E(g,C(211,149,93),-8,-147,69,62);E(g,C(237,198,143),29,-117,55,27);Eye(g,38,-122,t,1);E(g,C(81,62,49),73,-109,14,13);
        float chew=graze*Math.Abs(S(t*4.1))*3;L(g,C(129,89,61),2,new(60,-90),new(76,-89+chew));g.Restore(neck);g.Restore(st);
    }
    static void Hedgehog(Graphics g,float x,float y,float scale,double t)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);
        E(g,Color.FromArgb(28,46,84,52),-49,17,118,14);
        for(int i=0;i<24;i++){float a=MathF.PI+i*MathF.PI/23;float r=52+i%3*4;P(g,i%2==0?C(131,93,67):C(161,119,79),new(Cos(a)*r,S(a)*r*.64f),new(Cos(a+.085f)*(r+12),S(a+.085f)*(r+12)*.72f),new(Cos(a+.17f)*r,S(a+.17f)*r*.64f));}
        E(g,C(133,97,70),-52,-39,105,61);E(g,C(221,174,120),18,-23,64,45);P(g,C(221,174,120),new(64,-10),new(98,7),new(65,18));E(g,C(79,61,51),89,2,11,10);
        E(g,C(238,196,143),31,-34,18,19);Eye(g,60,-10,t,.74f);E(g,C(229,144,140),65,0,13,7);
        for(int i=0;i<3;i++)E(g,C(137,101,72),-25+i*34,14+S(t*4+i*2)*2,16,11);g.Restore(st);
    }
    static void Meadow(Graphics g,double t)
    {
        Sky(g,C(135,197,223),C(252,238,188));E(g,Color.FromArgb(170,255,243,174),1294,71,145,145);
        for(int i=0;i<5;i++)Cloud(g,Travel(t,9+i*1.8f,300+i*360),137+i%3*72,.7f+i*.08f);
        Hill(g,C(145,177,146),452,53,2);Hill(g,C(150,189,125),575,46,.5);Hill(g,C(126,177,108),685,37,2.5);Hill(g,C(177,202,126),801,14,1);
        // Far hedges and orchard trees add depth behind the animal clearing.
        for(int i=0;i<6;i++){float x=185+i*256;L(g,C(145,130,87),9,new(x,548),new(x+4,381));E(g,C(129,171,122),x-49,340,112,141);E(g,C(164,196,140),x-37,329,90,96);}
        for(int i=0;i<35;i++)Flower(g,17+i*48,653+i%5*21,26+i%3*13,i%3==0?C(238,210,164):C(242,235,206),t,i);
        Deer(g,1185,710,1.28f,t);Deer(g,1430,655,.64f,t+9);
        // A bunny makes a long oval circuit and hops only while moving.
        float p=Phase(t+9,61),angle=p*MathF.PI*2,rabbitX=680+Cos(angle)*242,rabbitY=717+S(angle)*52;
        Rabbit(g,rabbitX,rabbitY-Math.Abs(S(t*3.9))*10,.73f,t+3,false,S(angle)>0);
        Rabbit(g,570,797,1.2f,t,true,true);Rabbit(g,801,821,.7f,t+5,true);
        // The hedgehog strolls all the way across, with no on-screen reversal.
        Hedgehog(g,Travel(t,14,1010,1930),844,.87f,t);
        for(int i=0;i<36;i++)Flower(g,20+i*46,915-i%4*10,43+i%5*17,i%3==0?C(229,168,183):i%3==1?C(243,242,211):C(202,170,218),t,i+47);
        for(int i=0;i<4;i++)Butterfly(g,Travel(t,17+i*6,90+i*428),340+i%2*174+S(t*.43+i*2)*73,.66f+i*.05f,t+i*1.9,i%2==0?C(255,204,107):C(224,160,194));
        // Bees wander from flower to flower on distinct, unhurried paths.
        for(int i=0;i<4;i++)
        {
            float bx=220+i*352+S(t*.29+i*2)*64,by=681+i%2*69+S(t*.61+i)*28;var bee=g.Save();g.TranslateTransform(bx,by);g.RotateTransform(S(t*.7+i)*12);
            float wing=4+Math.Abs(S(t*18+i))*9;E(g,Color.FromArgb(135,247,253,240),-13,-wing-7,15,wing+5);E(g,Color.FromArgb(135,247,253,240),0,-wing-8,15,wing+6);
            E(g,C(251,205,97),-15,-6,30,15);R(g,C(111,89,62),-8,-5,4,13);R(g,C(111,89,62),1,-6,4,14);Eye(g,11,-2,t,.38f);g.Restore(bee);
        }
        // Soft travelling seeds and continuously swaying stems finish the foreground.
        for(int i=0;i<11;i++){float xx=Travel(t,11+i%3*4,i*171),yy=492+i%5*62+S(t*.37+i)*17;E(g,Color.FromArgb(160,255,249,217),xx,yy,5,5);L(g,Color.FromArgb(95,255,249,217),1,new(xx+2,yy),new(xx+S(t+i)*7,yy-9));}
    }
}
