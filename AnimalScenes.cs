using System.Drawing.Drawing2D;

namespace LiveDesktop;

static class AnimalScenes
{
    static Color C(int r,int g,int b) => Color.FromArgb(r,g,b);
    static void E(Graphics g,Color c,float x,float y,float w,float h) { using var b=new SolidBrush(c); g.FillEllipse(b,x,y,w,h); }
    static void R(Graphics g,Color c,float x,float y,float w,float h) { using var b=new SolidBrush(c); g.FillRectangle(b,x,y,w,h); }
    static void L(Graphics g,Color c,float width,float x,float y,float x2,float y2) { using var p=new Pen(c,width){StartCap=LineCap.Round,EndCap=LineCap.Round}; g.DrawLine(p,x,y,x2,y2); }
    static void P(Graphics g,Color c,params PointF[] pts) { using var b=new SolidBrush(c);g.FillPolygon(b,pts); }
    static void BG(Graphics g,Color a,Color b) {using var br=new LinearGradientBrush(new Rectangle(0,0,1600,900),a,b,90f);g.FillRectangle(br,0,0,1600,900);}
    static void Eye(Graphics g,float x,float y,float s=1) { E(g,C(32,42,59),x,y,12*s,17*s);E(g,Color.White,x+2*s,y+2*s,4*s,5*s); }
    static void Smile(Graphics g,float x,float y,float w=25) {using var p=new Pen(C(65,56,76),3);g.DrawArc(p,x,y,w,17,10,160);}
    static void Star(Graphics g,Color c,float x,float y,float r) { P(g,c,new(x,y-r),new(x+r*.25f,y-r*.25f),new(x+r,y),new(x+r*.25f,y+r*.25f),new(x,y+r),new(x-r*.25f,y+r*.25f),new(x-r,y),new(x-r*.25f,y-r*.25f)); }
    static void Cloud(Graphics g,float x,float y,float s,Color c) {E(g,c,x,y,180*s,40*s); E(g,c,x+24*s,y-25*s,70*s,65*s);E(g,c,x+83*s,y-15*s,70*s,53*s);}
    public static void Draw(Graphics g,int theme,double t)
    { switch(theme) {case 8:Axolotl(g,t);break;case 9:Bunny(g,t);break;case 10:Dragon(g,t);break;case 11:Panda(g,t);break;case 12:Ducks(g,t);break;} }
    static void Axolotl(Graphics g,double t)
    {
        BG(g,C(23,68,98),C(55,159,159));
        for(int i=0;i<6;i++) P(g,Color.FromArgb(12,218,255,231),new(100+i*290,0),new(180+i*290,0),new(600+i*260,900),new(310+i*260,900));
        for(int i=0;i<23;i++) {float x=40+i*71;float h=100+i%5*38;float sway=(float)Math.Sin(t*1.2+i)*22;L(g,C(41,128,123),12,x,870,x+sway,870-h);E(g,C(62,183,144),x+sway-12,840-h,30,70);}
        for(int i=0;i<6;i++) {float x=170+i*270;float y=785+i%2*30;E(g,C(63,125,135),x,y,170,75); E(g,C(129,172,162),x+18,y+8,85,15);}
        for(int i=0;i<28;i++){float x=35+(i*131)%1530+(float)Math.Sin(t+i)*13;float y=900-(float)((t*(20+i%5*6)+i*77)%1020);using var pen=new Pen(Color.FromArgb(95,193,253,248),2);g.DrawEllipse(pen,x,y,12+i%4*9,12+i%4*9);E(g,Color.FromArgb(125,240,255,250),x+4,y+3,4,4);}
        float bx=800+(float)Math.Sin(t*.35)*240,by=440+(float)Math.Sin(t*1.5)*27;var st=g.Save();g.TranslateTransform(bx,by);g.RotateTransform((float)Math.Sin(t*.8)*4);
        using(var path=new GraphicsPath()){path.AddBezier(85,32,240,-100,360,55,112,85);using var b=new SolidBrush(C(245,153,180));g.FillPath(b,path);}
        E(g,C(249,176,192),-105,3,240,100); E(g,C(255,214,218),-68,17,150,65);
        for(int side=-1;side<=1;side+=2)for(int j=0;j<3;j++){float x=side*101,y=-65+j*34;L(g,C(239,122,161),15,side*82,y+15,side*(150+j%2*13),y-20+j*12);E(g,C(255,166,199),x+side*43-12,y-35+j*12,25,36);}
        E(g,C(255,187,207),-108,-104,215,146);E(g,C(255,219,226),-82,-89,163,110);
        Eye(g,-54,-42,1.1f);Eye(g,41,-42,1.1f); E(g,C(247,142,174),-76,-17,32,15); E(g,C(247,142,174),48,-17,32,15);Smile(g,-15,-23,29);
        for(int side=-1;side<=1;side+=2) {E(g,C(255,185,206),side*72-20,63,42,29);L(g,C(245,154,183),4,side*73,82,side*73,90);}
        g.Restore(st);
    }
    static void Bunny(Graphics g,double t)
    {
        BG(g,C(17,22,55),C(66,43,104));
        var rng=new Random(812);for(int i=0;i<110;i++){float x=rng.Next(1600),y=rng.Next(900);float r=1+i%3;E(g,Color.FromArgb((int)(125+100*Math.Sin(t*.9+i)),230,225,255),x,y,r,r);if(i%18==0)Star(g,C(244,226,175),x,y,7);}
        E(g,C(105,88,172),1170,118,210,210);E(g,C(128,111,189),1204,139,70,45);E(g,C(84,68,145),1270,240,52,37);
        var pl=g.Save();g.TranslateTransform(300,225);g.RotateTransform(-20);using(var p=new Pen(C(208,164,178),18))g.DrawEllipse(p,-176,-40,352,80);E(g,C(234,171,160),-87,-87,174,174);E(g,C(248,192,174),-71,-72,87,54);g.Restore(pl);
        float mx=(float)((t*160)%2300)-500;L(g,Color.FromArgb(90,196,224,255),3,mx-180,70,mx,130);Star(g,C(231,244,255),mx,130,7);
        float x0=810+(float)Math.Sin(t*.35)*190,y0=457+(float)Math.Sin(t*.85)*35;var st=g.Save();g.TranslateTransform(x0,y0);g.RotateTransform((float)Math.Sin(t*.6)*8);
        using(var cord=new Pen(C(171,171,214),5)){g.DrawBezier(cord,-67,94,-260,195,-400,-100,-470,20);}
        E(g,C(181,194,232),-75,33,160,196);E(g,C(242,244,252),-63,29,133,174);E(g,C(201,217,245),-92,77,42,104);E(g,C(201,217,245),56,65,45,108);E(g,C(231,235,251),-67,200,60,44);E(g,C(231,235,251),20,200,60,44);
        E(g,C(216,227,248),-118,-162,236,225);E(g,C(104,122,177),-104,-146,208,194);E(g,C(171,207,230),-99,-141,198,184);
        E(g,C(249,241,243),-67,-124,43,106);E(g,C(249,241,243),23,-124,43,106);E(g,C(234,168,188),-55,-112,18,77);E(g,C(234,168,188),35,-112,18,77);E(g,C(255,246,243),-69,-58,139,110);
        Eye(g,-37,-18);Eye(g,27,-18);E(g,C(235,155,178),-7,3,15,11);Smile(g,-13,8,26);E(g,Color.FromArgb(120,255,255,255),-87,-110,26,78);
        R(g,C(135,169,219),-29,88,59,48);E(g,C(111,237,216),-17,100,13,13);E(g,C(243,188,120),5,100,13,13);g.Restore(st);
    }
    static void Island(Graphics g,float x,float y,float s,double t)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(s,s);
        P(g,C(87,105,149),new(-180,0),new(160,0),new(110,85),new(55,170),new(-33,116),new(-111,85));P(g,C(65,82,128),new(10,18),new(160,0),new(55,170));
        E(g,C(84,164,139),-180,-25,350,62);E(g,C(126,207,158),-165,-31,311,45);
        for(int i=0;i<4;i++){float ax=-110+i*58;E(g,C(220,157,193),ax,-36,16,12);L(g,C(64,130,109),3,ax+8,-30,ax+8,-17);}
        for(int i=0;i<3;i++){float ax=25+i*17;L(g,Color.FromArgb(110,184,241,242),8,ax,23,ax+(float)Math.Sin(t+i)*6,215+i*16);}
        L(g,C(104,97,124),12,-65,-15,-61,-89);E(g,C(150,191,198),-110,-145,100,81);E(g,C(178,212,213),-96,-156,88,63);g.Restore(st);
    }
    static void Dragon(Graphics g,double t)
    {
        BG(g,C(157,171,223),C(248,204,206)); E(g,C(252,231,185),1180,80,148,148);
        for(int i=0;i<7;i++)Cloud(g,(float)((i*300+t*12)%1900)-230,130+i%3*155,.7f+i%3*.3f,Color.FromArgb(90,255,250,245));
        Island(g,280,350,1.0f,t);Island(g,1300,530,1.1f,t);Island(g,510,790,.6f,t);
        var st=g.Save();g.TranslateTransform(805+(float)Math.Sin(t*.38)*190,410+(float)Math.Sin(t*1.4)*24);g.RotateTransform((float)Math.Sin(t)*3);
        float flap=(float)Math.Sin(t*4)*30;
        P(g,C(96,168,177),new(-15,20),new(-163,-110-flap),new(-180,-10-flap),new(-127,-18),new(-102,35),new(-52,8));P(g,C(178,222,217),new(-34,11),new(-155,-89-flap),new(-129,-14),new(-100,18));
        using(var path=new GraphicsPath()){path.AddBezier(52,45,140,180,242,48,202,8);path.AddBezier(202,8,219,114,141,101,65,7);using var b=new SolidBrush(C(87,170,160));g.FillPath(b,path);}
        E(g,C(104,194,173),-67,-16,156,145);E(g,C(210,235,187),-28,12,80,109);
        E(g,C(97,192,172),-47,-103,139,134);E(g,C(128,213,185),27,-32,92,58);
        P(g,C(242,229,173),new(-32,-87),new(-34,-138),new(-6,-102));P(g,C(242,229,173),new(56,-88),new(74,-133),new(77,-72));
        Eye(g,22,-59,1.2f);E(g,C(79,130,129),96,-12,7,8);Smile(g,62,1,29);E(g,C(236,168,171),48,-19,24,13);
        for(int i=0;i<4;i++)P(g,C(232,219,153),new(63+i*27,46+i*8),new(81+i*27,27+i*5),new(88+i*27,59+i*3));
        E(g,C(81,163,151),-64,81,55,34);E(g,C(81,163,151),38,91,55,34);g.Restore(st);
        for(int i=0;i<16;i++)Star(g,Color.FromArgb(95,255,246,209),80+i*96,590+(float)Math.Sin(i+t*.4)*70,3+i%3);
    }
    static void Panda(Graphics g,double t)
    {
        BG(g,C(160,204,172),C(226,237,188));E(g,Color.FromArgb(150,255,248,199),1170,86,190,190);
        for(int i=0;i<22;i++){float x=i*79-30;float h=380+i%5*83;float sway=(float)Math.Sin(t*.7+i)*7;Color c=i%2==0?C(73,130,108):C(106,160,116);L(g,c,18,x,900,x+sway,900-h);for(int j=0;j<5;j++){float y=900-j*95;L(g,C(153,183,120),3,x-7,y,x+7,y);if(j>1){L(g,c,5,x,y,x+60,y-55);E(g,c,x+30,y-72,63,15);L(g,c,5,x,y,x-50,y-40);E(g,c,x-82,y-61,58,14);}}}
        E(g,C(94,152,123),-100,720,1800,380);E(g,C(147,188,133),-100,795,1800,220);
        for(int i=0;i<25;i++){float x=(float)((i*119+t*(14+i%4))%1750)-75;float y=150+i%7*105+(float)Math.Sin(t+i)*16;var a=g.Save();g.TranslateTransform(x,y);g.RotateTransform((float)Math.Sin(t+i)*45);E(g,C(211,224,150),0,0,24,9);g.Restore(a);}
        E(g,Color.FromArgb(30,28,70,48),650,770,350,52);
        for(int i=0;i<3;i++){float bx=390+i*420+(float)Math.Sin(t*.22+i*2)*120,by=355+i%2*70+(float)Math.Sin(t*.48+i)*40;Butterfly(g,bx,by,.8f+i*.12f,t+i);}
        float breath=(float)Math.Sin(t*1.25)*.009f;
        var st=g.Save();g.TranslateTransform(825,780);g.ScaleTransform(1+breath,1+breath);g.TranslateTransform(0,-90);g.RotateTransform((float)Math.Sin(t*.4)*.7f);
        E(g,C(36,61,58),-110,56,105,65);E(g,C(36,61,58),22,56,105,65);E(g,C(245,242,217),-107,-142,230,239);E(g,C(35,58,56),-138,-89,74,173);E(g,C(35,58,56),80,-70,65,154);E(g,C(246,246,224),-135,-245,55,56);E(g,C(35,58,56),-127,-237,52,53);E(g,C(35,58,56),73,-241,60,60);E(g,C(254,250,231),-122,-226,248,188);
        var e1=g.Save();g.TranslateTransform(-55,-136);g.RotateTransform(25);E(g,C(41,63,61),-24,-34,52,65);g.Restore(e1);var e2=g.Save();g.TranslateTransform(57,-136);g.RotateTransform(-25);E(g,C(41,63,61),-24,-34,52,65);g.Restore(e2);
        E(g,Color.White,-60,-143,12,17);E(g,Color.White,51,-143,12,17);E(g,C(32,50,49),-16,-102,34,24);Smile(g,-24,-82+(float)Math.Sin(t*3.2)*2,48);E(g,C(239,181,165),-88,-98,32,16);E(g,C(239,181,165),60,-98,32,16);
        var snack=g.Save();g.TranslateTransform(-58,10);g.RotateTransform((float)Math.Sin(t*1.4)*2);g.TranslateTransform(58,-10);
        L(g,C(74,143,90),13,-96,15,100,-187);for(int i=0;i<4;i++){float x=-40+i*34,y=-47-i*35;E(g,C(108,174,94),x-6,y-26,64,17);}E(g,C(35,58,56),-89,-4,61,45);g.Restore(snack);g.Restore(st);
        for(int i=0;i<9;i++){float x=100+i*175;L(g,C(91,152,101),3,x,873,x-12,842);L(g,C(91,152,101),3,x,873,x+12,849);}
    }
    static void Butterfly(Graphics g,float x,float y,float scale,double t)
    {
        var st=g.Save();g.TranslateTransform(x,y);g.ScaleTransform(scale,scale);g.RotateTransform((float)Math.Sin(t*.7)*12);
        float w=8+(float)(.5+.5*Math.Sin(t*7))*13;
        E(g,C(244,187,142),-w,-20,w,25);E(g,C(244,187,142),0,-20,w,25);
        E(g,C(235,157,158),-w*.8f,0,w*.8f,17);E(g,C(235,157,158),0,0,w*.8f,17);
        L(g,C(89,90,75),3,0,-12,0,13);L(g,C(89,90,75),1,0,-11,-5,-18);L(g,C(89,90,75),1,0,-11,5,-18);g.Restore(st);
    }
    static PointF Swim(double t) => new(800+(float)Math.Sin(t*.07)*420,550+(float)Math.Cos(t*.07)*155);
    static void Duck(Graphics g,float x,float y,float s,double t,bool baby)
    {
        // World position must never feed the bob phase. Heading blends continuously through a turn.
        float heading=(float)Math.Tanh(Math.Cos(t*.07)*3);
        var st=g.Save();g.TranslateTransform(x,y+(float)Math.Sin(t*1.7)*3);g.ScaleTransform(s,s);
        using(var p=new Pen(Color.FromArgb(80,212,247,224),2))g.DrawEllipse(p,-94,45,188,19);
        double ripple=(t%4+4)%4/4;
        using(var p=new Pen(Color.FromArgb((int)(55*(1-ripple)),218,249,227),1.5f))g.DrawEllipse(p,-95-(float)ripple*35,43-(float)ripple*6,190+(float)ripple*70,22+(float)ripple*12);
        Color body=baby?C(255,220,115):C(250,243,215);E(g,body,-82,-18,169,87);P(g,body,new(-heading*65,15),new(-heading*108,-10),new(-heading*76,48));
        var wing=g.Save();g.TranslateTransform(-heading*10,19);g.RotateTransform((float)Math.Sin(t*1.1)*3);E(g,baby?C(245,190,82):C(224,219,185),-45,-18,89,46);g.Restore(wing);
        float nod=(float)(Math.Pow(.5+.5*Math.Sin(t*.55),12)*8);
        var head=g.Save();g.TranslateTransform(heading*58,-44+nod);g.RotateTransform(heading*(float)Math.Sin(t*.7)*2);
        E(g,body,-38,-39,78,92);P(g,C(239,157,62),new(heading*24,-1),new(heading*68,13),new(heading*30,28));
        Eye(g,heading*17-5,-13,.75f);E(g,C(243,180,128),heading*7-8,11,18,11);g.Restore(head);g.Restore(st);
    }
    static void Ducks(Graphics g,double t)
    {
        BG(g,C(146,205,198),C(68,151,157));
        E(g,C(121,166,116),-100,-150,1800,460);E(g,C(177,202,125),-100,-120,1800,360);
        for(int i=0;i<33;i++){float x=i*52;L(g,C(94,153,112),4,x,230,x+(float)Math.Sin(t+i)*9,160+i%3*17);}
        for(int i=0;i<36;i++){float x=(i*213)%1600,y=340+i%8*70+(float)Math.Sin(t*.9+i)*5;L(g,Color.FromArgb(55,215,245,220),2,x,y,x+47,y);}
        for(int i=0;i<4;i++){double life=((t+i*3.7)%13+13)%13/13;float radius=5+(float)life*23;int alpha=(int)(38*Math.Sin(life*Math.PI));using var p=new Pen(Color.FromArgb(alpha,231,249,232),1.5f);g.DrawEllipse(p,210+i*325-radius,410+i%2*310-radius*.3f,radius*2,radius*.6f);}
        for(int i=0;i<13;i++){float x=85+(i*293)%1490,y=380+(i*127)%470;var st=g.Save();g.TranslateTransform(x,y+(float)Math.Sin(t*.8+i)*4);g.RotateTransform((float)Math.Sin(t*.5+i)*4);E(g,C(40,117,98),-48,-16,99,38);P(g,C(68,150,111),new(0,3),new(47,-11),new(15,-17));if(i%3==0){for(int j=0;j<7;j++){var fl=g.Save();g.RotateTransform(j*51);E(g,C(248,173,195),-9,-35,18,37);g.Restore(fl);}E(g,C(248,213,127),-9,-9,18,18);}g.Restore(st);}
        PointF parent=Swim(t);Duck(g,parent.X,parent.Y,1.35f,t,false);
        for(int i=0;i<3;i++){double lag=t-7-i*5.8;PointF pt=Swim(lag);float drift=(float)Math.Sin(t*.6+i*2)*10;Duck(g,pt.X,pt.Y+18+i*13+drift,.68f-i*.055f,lag,true);}
        for(int i=0;i<3;i++){float x=1030+i*140+(float)Math.Sin(t*.25+i)*115,y=315+(float)Math.Cos(t*.43+i)*43;L(g,C(72,117,115),2,x-16,y,x+17,y);E(g,Color.FromArgb(115,239,249,236),x-21,y-8-(float)Math.Sin(t*12+i)*3,23,8);E(g,Color.FromArgb(115,239,249,236),x+1,y-8+(float)Math.Sin(t*12+i)*3,23,8);E(g,C(72,117,115),x-4,y-3,9,7);}
    }
}
