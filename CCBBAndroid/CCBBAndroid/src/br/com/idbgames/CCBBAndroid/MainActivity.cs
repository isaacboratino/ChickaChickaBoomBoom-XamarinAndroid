using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Util;
using static Android.Views.View;
using System.Threading;
using CCBBAndroid;

namespace br.com.idbgames.CCBBAndroid
{
    [Activity(Label = "Chicka Chicka Boom Boom", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class MainActivity : Activity, IOnTouchListener
    {
        protected int layoutWidth;
        protected int layoutHeight;
        protected int letterWidth;
        protected int letterHeight;
        protected int videoMusicaSelecionada;
        protected int[] lettersTelaArr;

        protected int[] imagesSpinnerArr = {Resource.Drawable.ccbb1, Resource.Drawable.ccbb2,
                                            Resource.Drawable.ccbb3, Resource.Drawable.ccbb4};

        protected string[] labelsSpinnerArr = { "1", "2", "3", "4" };

        protected Spinner mySpinner;        
        protected MainActivity main;

        // referencia ao layout mprincipal
        protected RelativeLayout layoutPrincipal;

        protected PlayMusicVids playMusicVids;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            main = this;

            layoutPrincipal = FindViewById<RelativeLayout>(Resource.Id.layoutPrincipal);

            // Fiz isso para pegar o tamanho real da tela
            layoutPrincipal.Post(() =>
            {
                Rect rect = new Rect();
                Window win = this.Window;  // Get the Window
                win.DecorView.GetWindowVisibleDisplayFrame(rect);

                // Get the height of Status Bar 
                int statusBarHeight = rect.Top;

                // Get the height occupied by the decoration contents 
                int contentViewTop = win.FindViewById(Window.IdAndroidContent).Top;

                // Calculate titleBarHeight by deducting statusBarHeight from contentViewTop  
                int titleBarHeight = contentViewTop - statusBarHeight;

                // By now we got the height of titleBar & statusBar
                // Now lets get the screen size
                DisplayMetrics metrics = new DisplayMetrics();
                this.WindowManager.DefaultDisplay.GetMetrics(metrics);
                int screenHeight = metrics.HeightPixels;
                int screenWidth = metrics.WidthPixels;

                // Now calculate the height that our layout can be set
                // If you know that your application doesn't have statusBar added, then don't add here also. Same applies to application bar also 
                layoutHeight = screenHeight - (titleBarHeight + statusBarHeight);
                layoutWidth = screenWidth;

                // Calculo 13% do tamanho da tela para ser o tamanho de cada letra
                letterWidth = (int)layoutWidth * 15 / 100;
                letterHeight = (int)layoutHeight * 15 / 100;

                // Array com todas as letras que serao arrastaveis na tela
                int[] lettersImgArr = { Resource.Drawable.an,Resource.Drawable.bn,Resource.Drawable.cn,
                                        Resource.Drawable.dn,Resource.Drawable.en,Resource.Drawable.fn,
                                        Resource.Drawable.gn,Resource.Drawable.hn,Resource.Drawable.fn,
                                        Resource.Drawable.jn,Resource.Drawable.kn,Resource.Drawable.ln,
                                        Resource.Drawable.mn,Resource.Drawable.nn,
                                        Resource.Drawable.on,Resource.Drawable.pn,Resource.Drawable.qn,
                                        Resource.Drawable.rn,Resource.Drawable.sn,Resource.Drawable.tn,
                                        Resource.Drawable.un,Resource.Drawable.vn,Resource.Drawable.wn,
                                        Resource.Drawable.xn,Resource.Drawable.yn,Resource.Drawable.zn};

                // Defino array que ira conter todas as letras adicionadas na tela
                // para poder fazer a caida delas para o solo novamente, no chicka chicka boom boom
                lettersTelaArr = new int[lettersImgArr.Length];

                // Coloco todas as letras na tela para serem arrastadas
                GenerateLettersTouch(layoutPrincipal, lettersImgArr);

                //// Define o spinner
                mySpinner = (Spinner)FindViewById(Resource.Id.videoMusicaSpinner);
                mySpinner.Adapter = (new MySpinnerAdapter(this, imagesSpinnerArr));

                //// Faz o load das musicas e sons e prepara o player de video
                playMusicVids = new PlayMusicVids(main);
                //PlayMusica("mccbb1");

                // Seta o evento de seleção
                mySpinner.ItemSelected += (sender, args) =>
                {
                    videoMusicaSelecionada = 0;
                    if (args.Position > -1)
                    {
                        videoMusicaSelecionada = args.Position + 1;
                        PlayMusica("mccbb" + videoMusicaSelecionada);
                    }
                };
            });	

            Button playMusicaButton = FindViewById<Button>(Resource.Id.playMusicaButton);
            playMusicaButton.Click += btPlayMusica_onClick;

            Button playVideoButton = FindViewById<Button>(Resource.Id.playVideoButton);
            playVideoButton.Click += btPlayVideo_onClick;

            Button chickaChickaButton = FindViewById<Button>(Resource.Id.chickaChickaButton);
            chickaChickaButton.Click += chickaBoomButton_onClick;
        }

        private void GenerateLettersTouch(RelativeLayout rl, int[] lettersImgArr)
        {

            // Varro o array de letras para pintar cada uma delas na tela
            for (int i = lettersImgArr.Length - 1; i >= 0; i--)
            {
                ImageView imgView = new ImageView(this);
                imgView.Id = (12 + i);
                imgView.SetImageResource(lettersImgArr[i]); // seta a imagem na imageview

                RelativeLayout.LayoutParams parms = new RelativeLayout.LayoutParams(letterWidth, letterHeight);
                imgView.LayoutParameters = parms;

                // defino o posicionamento da letra na tela, apenas na região baixa da tela.
                PosicionaLetraTela(imgView);

                // torna a letra arrastavel
                imgView.SetOnTouchListener(this);

                // torna a imagem visivel
                imgView.Visibility = ViewStates.Visible;

                // adiciono ela ao layout principal
                rl.AddView(imgView);

                // adiciono lettra no array, para depois poder dar respaw ou boom boom
                lettersTelaArr[i] = imgView.Id;
            }

        }

        private void PosicionaLetraTela(ImageView imgView)
        {
            Random random = new Random();

            float yPercent = layoutHeight * 15 / 100;
            float minY = (layoutHeight - yPercent) - yPercent;
            float maxY = layoutHeight - yPercent;

            float finalY = (float)(minY + random.NextDouble() * (maxY - minY));

            // defino a região onde as letras devem aparecer
            float xPercent = layoutWidth * 10 / 100;
            float minX = xPercent;
            float maxX = (layoutWidth - imgView.Width) - xPercent;

            float finalX = (float)(minX + random.NextDouble() * (maxX - minX));

            imgView.Animate().TranslationX(finalX).WithLayer();
            imgView.Animate().TranslationY(finalY).WithLayer();

            //imgView.SetX(finalX);
            //imgView.SetY(finalY);

        }

        float x, y = 0.0f;
        bool moving = false;

        public bool OnTouch(View v, MotionEvent e)
        {

            switch (e.Action)
            {
			    case MotionEventActions.Down:
				    moving = true;
                break;
				
			    case MotionEventActions.Move:
				    if (moving)
                    {
                        x = e.RawX-v.Width/2;
                        y = (e.RawY-v.Height*3/2);
                        v.SetX(x);
                        v.SetY(y);
                    }
				    break;
				
			    case MotionEventActions.Up:
				    moving = false;
				    break;
		    }
		    return true;
        }

        public void btPlayMusica_onClick(object sender, EventArgs e)
        {
            //playMusicVids.playBeep("ccbb-explosion");
            PlayMusica("mccbb" + videoMusicaSelecionada);
        }

        private void PlayMusica(string nomeMusica)
        {
            Button btMusicaPlay = FindViewById<Button>(Resource.Id.playMusicaButton);
            Button btVideoPlay = FindViewById<Button>(Resource.Id.playVideoButton);

            if (btMusicaPlay.Text.Equals("Play"))
            {
                playMusicVids.stopAll();

                // Roda o video
                playMusicVids.playMusic("musicas", nomeMusica, "3gp");

                //troca o botão do player do video, para ser o stop do video			
                btMusicaPlay.Text = "Stop";
                btMusicaPlay.BringToFront();

                // seta o botão do video para play
                btVideoPlay.Text = "Video";
            }
            else
            {
                playMusicVids.stopAll();
                btMusicaPlay.Text = "Play";
            }
        }


        private void btPlayVideo_onClick(object sender, EventArgs e)
        {
            Button btVideoPlay = (Button)FindViewById(Resource.Id.playVideoButton);
            Button btMusicaPlay = (Button)FindViewById(Resource.Id.playMusicaButton);

            if (btVideoPlay.Text.Equals("Video"))
            {
                int idVideoSelecionado = 0;
                switch (videoMusicaSelecionada)
                {
                    case 1: idVideoSelecionado = Resource.Raw.ccbb1; break;
                    case 2: idVideoSelecionado = Resource.Raw.ccbb2; break;
                    case 3: idVideoSelecionado = Resource.Raw.ccbb3; break;
                    case 4: idVideoSelecionado = Resource.Raw.ccbb4; break;
                }

                playMusicVids.stopAll();

                // Roda o video
                playMusicVids.playVideo(string.Format("android.resource://{0}",this.GetType().Namespace),
                                        idVideoSelecionado,
                                        Resource.Id.videoPlayer);

                //troca o botão do player do video, para ser o stop do video			
                btVideoPlay.Text = "Fechar";
                btMusicaPlay.Text = "Play";
                btVideoPlay.BringToFront();
            }
            else
            {
                playMusicVids.stopAll();
                btVideoPlay.Text = "Video";
                btMusicaPlay.Text = "Play";
            }
        }

        public void chickaBoomButton_onClick(object sender, EventArgs e)
        {

            Button btVideoPlay = (Button)FindViewById(Resource.Id.playVideoButton);
            Button btMusicaPlay = (Button)FindViewById(Resource.Id.playMusicaButton);

            playMusicVids.stopAll();
            btVideoPlay.Text = "Video";
            btMusicaPlay.Text = "Play";

            playMusicVids.playMusic("musicas", "boom", "3gp");


            System.Threading.Tasks.Task.Factory.StartNew(() => {
                Thread.Sleep(7000);
                ReposicionaLetras();
            });

                    

            // Reposiciona as letras em seus lugares
            //SetTimeout(7000);
        }


        protected void ReposicionaLetras()
        {
            // Reposiciona as letras em seus lugares
            for (int i = 0; i < lettersTelaArr.Length; i++)
            {
                ImageView imgView = (ImageView)FindViewById(lettersTelaArr[i]);
                PosicionaLetraTela(imgView);
            }
        }
    }
}

