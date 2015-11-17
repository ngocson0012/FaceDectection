using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Diagnostics;
using System.Threading;
using DXApplication1.lib;

namespace DXApplication1.gui
{
    public partial class NhanDang : Form
    {
        public int HDfaces = 1;
        public int mauso = 1;
        public NhanDang()
        {
            InitializeComponent();

            InitGrid();
            try
            {

                face = new HaarCascade("Nariz_face.xml");
                eye = new HaarCascade("eye.xml");
                nose = new HaarCascade("Nariz_nuevo_20stages(nose).xml");
                mouth = new HaarCascade("Nariz_mouth.xml");
            }
            catch (Exception i) { MessageBox.Show(i.ToString()); }
            try
            {
                //Load of previus trainned faces and labels for each image
                // LoadImageAndnames();
            }
            catch (Exception es)
            {
                MessageBox.Show("Nothing in database, please add at least a face(Simply train the prototype with the AddFace Button)" + es.ToString(), "Triained faces load", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }


        }
        #region load image and name
        private void LoadImageAndnames()
        {
            //       string Labelsinfo = File.ReadAllText(Application.StartupPath + "/Faces/TrainedLabels.txt");
            //       string[] Labels = Labelsinfo.Split('%');

            //       NumLabels = Convert.ToInt16(Labels[0]);
            tong = 0;
            //       ContTrain = NumLabels;
            string LoadFaces = "";
            trainingImages = null;
            trainingImages = new List<Image<Gray, byte>>();
            labels = null;
            labels = new List<string>();

            // FileInfo[] Archivesnose = directory.GetFiles("nose*.bmp");
            foreach (FileInfo fileifo in Archives)
            {
                string[] name = fileifo.Name.Split('.');
                LoadFaces = name[0];

                if (LoadFaces.Substring(0, 4) == "face")
                {
                    tong++;
                    tf = Convert.ToInt16(LoadFaces.Substring(4));
                    int k = tf - Convert.ToInt16(tf.ToString().Substring(tf.ToString().Length - 1));

                    trainingImages.Add(new Image<Gray, byte>(directory + LoadFaces + ".bmp"));
                    labels.Add(kketnoi.lay1dong("select tensv from sinhvien sv, hinh h where sv.mssv=h.mssv and hinh='" + k + "'"));
                    Bitmap bmp = new Bitmap(directory + LoadFaces + ".bmp");
                    Bitmap tamnewsize = new Bitmap(bmp, newsizegb);
                    gabor(tamnewsize);
                    matrix1s.Add(x);
                    //  MessageBox.Show(directory + LoadFaces + ".bmp" + "\n" + k);
                    //  pictureBox_thu.Image = Image.FromFile(directory + LoadFaces);
                    //    Bitmap bmp = new Bitmap(Image.FromFile(directory + LoadFaces));
                    //  imb_thu.Image = new Image<Bgr, byte>(bmp);
                }
                else

                    if (LoadFaces.Substring(0, 4) == "nose")
                {
                    tf = Convert.ToInt16(LoadFaces.Substring(4, 1));
                    trainingImagenose.Add(new Image<Gray, byte>(directory + LoadFaces));
                    //   labelsnose.Add(kketnoi.lay1dong("select ten from sinhvien where hinh=" + tf + "or hinh=" + (tf - 1) + " or hinh=" + (tf - 2) + ""));
                }
                else
                        if (LoadFaces.Substring(0, 4) == "eyeL")
                {

                    tf = Convert.ToInt16(LoadFaces.Substring(4, 1));
                    trainingImageneyeL.Add(new Image<Gray, byte>(directory + LoadFaces));
                    //    labelseyeL.Add(kketnoi.lay1dong("select ten from sinhvien where hinh=" + tf + "or hinh=" + (tf - 1) + " or hinh=" + (tf - 2) + ""));
                }
                else
                            if (LoadFaces.Substring(0, 4) == "eyeR")
                {
                    tf = Convert.ToInt16(LoadFaces.Substring(4, 1));
                    trainingImageneyeR.Add(new Image<Gray, byte>(directory + LoadFaces));
                    //     labelseyeR.Add(kketnoi.lay1dong("select ten from sinhvien where hinh=" + tf + "or hinh=" + (tf - 1) + " or hinh=" + (tf - 2) + ""));
                }
                else
                //  if (LoadFaces.Substring(0, 4) == "mouth")
                {

                    tf = Convert.ToInt16(LoadFaces.Substring(5, 1));
                    trainingImagemouth.Add(new Image<Gray, byte>(directory + LoadFaces));
                    //     labelsmouth.Add(kketnoi.lay1dong("select ten from sinhvien where hinh=" + tf + "or hinh=" + (tf - 1) + " or hinh=" + (tf - 2) + ""));
                }





            }
            ContTrain = tf;

        }
        #endregion 

        private void loaddanhsachsv()
        {
            //MessageBox.Show("AAA");

            labels = null;
            matrix1s = null;
            labels = new List<string>();
            matrix1s = new List<Matrix1>();

            int len = newsizegb.Width; //MessageBox.Show("new size gb with" + len.ToString());
            dt = kketnoi.laydl("select hinh,h.MSSV from Hinh H,sinhvien sv where h.mssv=sv.mssv and malop='" + loptxt.Text.Trim() + "'");
            int indexmt; string dtt2 = "";//============================
            for (int tbd = 0; tbd < dt.Rows.Count; tbd++)
            {
                //MessageBox.Show(dt.Rows.Count.ToString());
                string mangmatran = dt.Rows[tbd]["hinh"].ToString();
                //MessageBox.Show(mangmatran.Length.ToString() +"\n"+mangmatran.ToString());

                //for (int i = 0; i < mangmatran.Length - 1; i += 2)
                //{
                string[] mangmatran1 = mangmatran.Split('.');

                // string s = "";
                //dtt2 = "";
                x = new Matrix1(len, len);
                indexmt = 0;
                for (int j = 0; j < len; j++)
                    for (int k = 0; k < len; k++)
                    {
                        x[j, k] = Convert.ToInt16(mangmatran1[indexmt]);//mangmatran[i][indexmt] - 48;
                        dtt2 += x[j, k];
                        indexmt++;
                        //s += x[j, k];
                    }
                // MessageBox.Show(dtt2);
                labels.Add(dt.Rows[tbd]["MSSV"].ToString());

                matrix1s.Add(x);



                // }
                // dtt2 += x.ToString();//=======================

            }
            //string fileName = Application.StartupPath + "\\dtt2.txt";
            //StreamWriter sw = new StreamWriter(fileName, false);
            //sw.WriteLine(dtt2);
            //sw.Close();

            // gridControl1.DataSource = dt;
        }
        ketnoi kketnoi = new ketnoi();
        DataTable dt = new DataTable();
        string[] tenanh = new string[3];
        List<string> imagearray = new List<string>();
        private int dem = 0, k = 0;

        string name1 = "", stt = "", directorypath = "";
        int demhinh, tf, tong, SoNguoi = 0;
        int yM = 0, xM = 0;
        double sf = 0;
        bool ktdslop = false;

        DirectoryInfo directory;
        FileInfo[] Archives;

        Rectangle rt = new Rectangle();
        Rectangle rte = new Rectangle();
        Rectangle rtm = new Rectangle();
        Rectangle rtn = new Rectangle();

        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade eye, nose, face, mouth;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> resultface, resulteyeL, resulteyeR, resultnose, resultmouth, TrainedFace = null;
        Image<Gray, byte> gray = null;
        Image<Gray, byte> grayf = null;
        Image<Gray, byte> graye = null;
        Image<Gray, byte> grayfm = null;
        Image<Gray, byte> graytam = null;
        Image<Gray, byte> grayfn = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<Image<Gray, byte>> trainingImagenose = new List<Image<Gray, byte>>();
        List<Image<Gray, byte>> trainingImageneyeL = new List<Image<Gray, byte>>();
        List<Image<Gray, byte>> trainingImageneyeR = new List<Image<Gray, byte>>();
        List<Image<Gray, byte>> trainingImagemouth = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();

        List<Image<Gray, byte>> tface = new List<Image<Gray, byte>>();

        List<string> tten = new List<string>();



        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        VideoWriter VW;
        string path = "",s="";
        List<string> hex = new List<string>();
        



        //BindingList<Person> gridDataList = new BindingList<Person>();
        void InitGrid()
        {

        }
        void FrameGrabber(object sender, EventArgs e)
        {
            try
            {
                //  NamePersons.Add("");

                currentFrame = grabber.QueryFrame().Resize(640, 480, INTER.CV_INTER_CUBIC);
                //Convert it to Grayscale
                gray = currentFrame.Convert<Gray, Byte>();
                //Face Detector
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
              face,
              1.1,
              5,
              Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
              new Size(60, 60));

                SoNguoi = 0;
                //Action for each element detected

                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    resultface = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                    sf = f.rect.Width / 100.0;

                    //draw the face detected in the 0th (gray) channel with blue color
                    currentFrame.Draw(f.rect, new Bgr(Color.Green), 2);

                    grayf = resultface.Resize(30, 30, INTER.CV_INTER_CUBIC);
                    Bitmap tam = grayf.ToBitmap();
                    //Bitmap tamnewsize = new Bitmap(tam, newsizegb);
                    matrixtam = PCA.image_2_matrix(tam);
                    matrixtam = Radon1.ApdungRadon(matrixtam);//PCA.apDungWaveletGabors(matrixtam, 0, 1.56, 1);
                                                              /////////////////////////////////////////

                    #region detect eye, nose, mouth

                    //phat hien mat
                    //eye detect        
                    grayf = resultface;


                    rte.X = 0; rte.Y = 15;
                    rte.Width = 100;
                    rte.Height = 40;
                    graye = grayf.Copy(rte).Convert<Gray, byte>();

                    MCvAvgComp[][] eyesDetected = graye.DetectHaarCascade(
                                                  eye,
                                                 1.02,
                                                  5,
                                                  Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                                                  new Size(20, 20));

                    int k = 0;
                    foreach (MCvAvgComp es in eyesDetected[0])
                    {

                        rt.X = (int)(sf * es.rect.X) + f.rect.X;
                        rt.Y = (int)(sf * (es.rect.Y + 15)) + (int)((f.rect.Y));
                        rt.Width = (int)(26 * sf);
                        rt.Height = (int)(26 * sf);


                        currentFrame.Draw(rt, new Bgr(Color.Yellow), 2);

                        rt.X = es.rect.X; rt.Y = es.rect.Y + 15;
                        rt.Width = 23;
                        rt.Height = 23;
                        graytam = grayf.Copy(rt).Convert<Gray, byte>();
                        if (rt.X > 50)
                        {
                            this.ibe1.Image = graytam;
                            resulteyeR = graytam;
                        }
                        else
                        if (rt.X <= 50)
                        {
                            this.ibe2.Image = graytam;
                            resulteyeL = graytam;
                        }

                        k++;
                        if (k == 2) break;

                    }

                    ////////////////////////////////////////////////////cat mouth tren grayface
                    rtm.X = 0; rtm.Y = 60;
                    rtm.Width = 100;
                    rtm.Height = 40;
                    grayfm = grayf.Copy(rtm).Convert<Gray, byte>();

                    ////////////////////////////////////////


                    //mouth detector
                    MCvAvgComp[][] mouthsDetected = grayfm.DetectHaarCascade(
                mouth,
                1.1,
                5,
                Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.FIND_BIGGEST_OBJECT,
                new Size(20, 20));
                    //  Console.WriteLine(mouthsDetected.Length);

                    foreach (MCvAvgComp m in mouthsDetected[0])
                    {

                        rt.X = (int)(m.rect.X * sf) - 2 + (int)(f.rect.X);
                        rt.Y = (int)((60 + m.rect.Y) * sf) + (int)(f.rect.Y);

                        rt.Width = (int)(40 * sf);
                        rt.Height = (int)(20 * sf);
                        currentFrame.Draw(rt, new Bgr(Color.Black), 2);
                        rt.X = m.rect.X; rt.Y = m.rect.Y + 60;
                        rt.Width = 40;
                        rt.Height = 20;
                        graytam = grayf.Copy(rt).Convert<Gray, byte>();
                        this.ibm.Image = graytam;
                        resultmouth = graytam;
                        break;
                    }

                    ////////////////////////////////////////////////////

                    ////////////////////////////////////////////////////cat mũi tren face

                    rtn.X = 0;
                    rtn.Y = 30;
                    rtn.Width = 100;
                    rtn.Height = 50;
                    grayfn = grayf.Copy(rtn).Convert<Gray, byte>();

                    //nose detect
                    MCvAvgComp[][] nosesDetected = grayfn.DetectHaarCascade(
              nose,
             1.1,
              5,
              Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.FIND_BIGGEST_OBJECT,
              new Size(20, 20));
                    foreach (MCvAvgComp n in nosesDetected[0])
                    {
                        rt.X = (int)(n.rect.X * sf) + f.rect.X + 5;
                        rt.Y = (int)((n.rect.Y + 28) * sf) + f.rect.Y;
                        rt.Width = (int)(30 * sf);
                        rt.Height = (int)(30 * sf);
                        currentFrame.Draw(rt, new Bgr(Color.Red), 2);
                        rt.X = n.rect.X; rt.Y = n.rect.Y + 30;
                        rt.Width = 30;
                        rt.Height = 30;
                        graytam = grayf.Copy(rt).Convert<Gray, byte>();
                        this.ibn.Image = graytam;
                        resultnose = graytam;
                        break;
                    }
                    //xx
                    #endregion

                    ////////////////////////////////////////////////// 
                    if (matrix1s != null)
                    {
                        int khs = Convert.ToInt16(txtk.Text);

                        List<double> Index_Maxtrix_Max = new List<double>();
                        string namegb = "", nameAvg = "";
                        double max = 0, avg = 0;
                        Matrix1.Compare(matrix1s, labels, matrixtam, out max, out namegb, out avg, out nameAvg, khs, out Index_Maxtrix_Max);
                        name1 = namegb;
                        lblTen.Text = namegb;
                        lblMax.Text = string.Format("{0:00.0000}", max);
                        lblAvg.Text = string.Format("{0:00.0000}", avg);
                        lblTenAvg.Text = nameAvg;
                        // string test = "";
                        if (Index_Maxtrix_Max != null)
                        {
                            //foreach (var ins in Index_Maxtrix_Max)
                            //{
                            //    test += ins.ToString() + "\n";
                            //}
                            // MessageBox.Show(test);
                            CapNhatAnh(Index_Maxtrix_Max, nameAvg, matrixtam);

                        }
                        //Draw the label for each face detected and recognized
                        currentFrame.Draw(namegb+" "+DateTime.Now.Second+" "+DateTime.Now.Millisecond, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));


                    }
                    //  NamePersons[t - 1] = name;
                    //  NamePersons.Add("");

                    //Set the number of faces detected on the scene
                    //                label3.Text = facesDetected[0].Length.ToString();

                    // break;
                    SoNguoi++;

                }
                t = 0;
                lblSoNguoi.Text = SoNguoi.ToString();
                //Names concatenation of persons recognized
                //for (int nnn = 0; nnn < facesDetected[0].Length; nnn++)
                //{
                //    names = names + NamePersons[nnn] + " ";
                //}
                //Show the faces procesed and recognized
                
                    LuuVideoMethod();
                    //Thread.Sleep(1000);
                
                imageBoxframgrabber.Image = currentFrame;//Console.WriteLine(DateTime.Now.Second+" "+DateTime.Now.Millisecond);

                if (comboBoxEdit1.Text != null && name1 != null)
                    diemdanh();

                //  names = "";
                //Clear the list(vector) of names
                //  NamePersons.Clear();
            }
            catch (Exception exx)
            {
                MessageBox.Show("Loi la \n " + exx.ToString());
            }
        }

        private void LuuVideoMethod()
        {
            //s = BitConverter.ToString(library.ConvertImageToByte(currentFrame.ToBitmap())); //.Replace("-", "");
            //hex.Add(s);
            if(luuvideo)
            VW.WriteFrame(currentFrame);

        }

        public void CapNhatAnh(List<double> list, string ma, Matrix1 matrix)
        {
            //Cập nhật số lần
            DataTable dtb = new DataTable();
            SqlCommand cm;
            // SqlDataAdapter da;
            string index = "";
            kketnoi.connect();
            for (int l = 0; l < list.Count; l += 2)
            {
                index += list[l].ToString();
                cm = new SqlCommand("UPDATE [Hinh] SET [SoLan] = @solan WHERE MSSV=@mssv and MaHinh=@mahinh", kketnoi.con);
                string tam = kketnoi.lay1dong("select solan from hinh where mssv='" + ma + "' and mahinh='" + list[l] + "' ");
                if (tam.Trim() == "") tam = "0";
                cm.Parameters.AddWithValue("@solan", Convert.ToInt32(tam) + 1);
                cm.Parameters.AddWithValue("@mssv", ma);
                cm.Parameters.AddWithValue("@mahinh", list[l]);
                kketnoi.connect();
                cm.ExecuteNonQuery();
            }
            kketnoi.connectClose();
            //Tìm kiếm mã ảnh có số lần cập nhật thấp nhất theo mssv
            string mahinh = kketnoi.lay1dong("select top 1  mahinh from hinh where mssv='" + ma + "' and solan<=(select min(solan) from hinh where mssv='" + ma + "')");
            // Lưu ma trận
            string matran = "";
            int row = matrix.NoRows;
            int col = matrix.NoCols;
            for (int k = 0; k < row; k++)
                for (int l = 0; l < col; l++)
                {
                    matran += matrix[k, l];
                    matran += '.';
                }

            string minsolan = kketnoi.lay1dong("select min(solan) from hinh where mssv='" + ma + "'");
            //MessageBox.Show("mssv " + ma + "_" + "\nmahinh it nhat " + mahinh);
            //MessageBox.Show(matran.ToString());

            //cập nhật ma trận +so lan theo mã hình và mssv
            kketnoi.connect();
            cm = new SqlCommand("UPDATE [Hinh] SET [Hinh] = @hinh, solan=@solan, ngaycapnhat=@ngay WHERE MSSV=@mssv and MaHinh=@mahinh", kketnoi.con);
            cm.Parameters.AddWithValue("@hinh", matran);
            cm.Parameters.AddWithValue("@mssv", ma);
            cm.Parameters.AddWithValue("@mahinh", mahinh);
            cm.Parameters.AddWithValue("@solan", Convert.ToInt16(minsolan) + 1);
            cm.Parameters.AddWithValue("@ngay", DateTime.Now.ToString());
            cm.ExecuteNonQuery();
            kketnoi.connectClose();
            //MessageBox.Show(index);
        }

        #region recognize by opencv (eigen face)...
        private string recognizerall(MCvAvgComp f)
        {

            string[] ten = new string[5];
            ten[0] = "";


            if (trainingImages.ToArray().Length != 0)
            {

                //  /Term Criteria for face recognition with numbers of trained images like max Iteration,eps > =>chinh xac
                MCvTermCriteria termCrit = new MCvTermCriteria(tong, 0.6);
                MCvTermCriteria termCritn = new MCvTermCriteria(tong, 0.7);
                MCvTermCriteria termCritm = new MCvTermCriteria(tong, 0.7);
                MCvTermCriteria termCriteL = new MCvTermCriteria(tong, 0.7);
                MCvTermCriteria termCriteR = new MCvTermCriteria(tong, 0.7);
                //Eigen face recognizer 

                EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                   trainingImages.ToArray(),
                   labels.ToArray(),
                   2000,
                   ref termCrit);

                ten[0] = recognizer.Recognize(resultface);
                /*
                
                 ///////////////////////////////////////////////////kiem tra nose/
                 if (resultnose != null)
                 {
                     EigenObjectRecognizer recognizernose = new EigenObjectRecognizer(
                        trainingImagenose.ToArray(),
                        labels.ToArray(),
                        1000,
                        ref termCritn);

                     ten[1] = recognizernose.Recognize(resultnose);
                     currentFrame.Draw("nose: "+ten[1], ref font, new Point(f.rect.X - 2, f.rect.Y - 15), new Bgr(Color.DarkBlue));
                
               
                 }
                 //////////////////////////////////////////////////////////
                
                 if (resultmouth != null)
                 {
                        EigenObjectRecognizer recognizermouth = new EigenObjectRecognizer( 
                        trainingImagemouth.ToArray(),
                        labels.ToArray(),
                        1000,
                        ref termCritm);

                     ten[2] = recognizermouth.Recognize(resultmouth);
                     currentFrame.Draw("mouth: "+ten[2], ref font, new Point(f.rect.X - 2, f.rect.Y - 30), new Bgr(Color.LightGreen));
                 }
 
                 if (resulteyeL != null)
                 {
                     EigenObjectRecognizer recognizereyeL = new EigenObjectRecognizer(
                     trainingImageneyeL.ToArray(),
                     labels.ToArray(),
                     1000,
                     ref termCriteL);

                     ten[3] = recognizereyeL.Recognize(resulteyeL);
                     currentFrame.Draw("eyes: "+ten[3], ref font, new Point(f.rect.X - 45, f.rect.Y - 45), new Bgr(Color.LightGreen));
                 }
                 if (resulteyeR != null)
                 {
                     EigenObjectRecognizer recognizereyeR = new EigenObjectRecognizer(
                     trainingImageneyeR.ToArray(),
                     labels.ToArray(),
                     600,
                     ref termCriteR);

                    ten[4] = recognizereyeR.Recognize(resulteyeR);
                    currentFrame.Draw(ten[4], ref font, new Point(f.rect.X +65, f.rect.Y - 45), new Bgr(Color.LightGreen));
                 }
               

               
             }
            
            
             int tam = 0;
             string name="";
             for (int i = 1; i < 5; i++)
             {
                 if (ten[0] == ten[i]) tam++;
                 if (tam > 2&&ten[0]!=null) { name = ten[0]; break; } else name = "";
             }
                 */
            }
            return ten[0];
        }
        #endregion

        bool ktgrabber;
        private void IPcamDetect()
        {
            try
            {
                if (!ktgrabber)
                {
                    detect.Text = "Stop";
                    ktgrabber = true;
                    grabber = new Capture("rtsp://192.168.0.10//user=admin1_password=admin1_channel=1_stream=0.sdp");
                    grabber.QueryFrame();
                    Application.Idle += new EventHandler(FrameGrabber);
                    addface.Enabled = true;

                    //cancle.Enabled = true;
                }
                else
                {
                    grabber.Dispose();
                    Application.Idle -= new EventHandler(FrameGrabber);
                    detect.Text = "Start";
                    ktgrabber = false;
                    addface.Enabled = false;
                    // cancle.Enabled = false;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void webCamDetect()
        {
            try
            {
                if (!ktgrabber)
                {
                    detect.Text = "Stop";
                    ktgrabber = true;
                    grabber = new Capture();
                    grabber.QueryFrame();
                    Application.Idle += new EventHandler(FrameGrabber);
                    addface.Enabled = true;
                    //cancle.Enabled = true;
                }
                else
                {
                    grabber.Dispose();
                    Application.Idle -= new EventHandler(FrameGrabber);
                    detect.Text = "Start";
                    ktgrabber = false;
                    addface.Enabled = false;
                    // cancle.Enabled = false;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void detect_Click(object sender, EventArgs e)
        {
            if (webCam.Checked == false && ipCam.Checked == false)
            {
                MessageBox.Show("Please select camera to use");
            }
            else
            {
                if (webCam.Checked == true)
                {
                    webCamDetect();
                }
                else
                {
                    IPcamDetect();
                }
            }


        }

        private void luuanh()
        {

            string matrananh = "";
            int row, col;
            string dtt1 = "";//==================================
            for (int i = 0; i < matrix1stam.Count; i++)
            {


                row = matrix1stam[i].NoRows;
                col = matrix1stam[i].NoCols;

                for (int k = 0; k < row; k++)
                    for (int l = 0; l < col; l++)
                    {
                        matrananh += matrix1stam[i][k, l];
                        dtt1 += matrix1stam[i][k, l];//==============================
                        matrananh += '.';

                    }
                kketnoi.connect();
                SqlCommand cm3 = new SqlCommand("insert into Hinh values('" + mssvtxt.Text.Trim() + "'," + i + ",'" + matrananh + "','','" + DateTime.Now.ToString() + "')", kketnoi.con); //son
                cm3.ExecuteNonQuery();
                matrananh = "";
                //matrananh += "@0@";
            }
            //===========================
            //MessageBox.Show(matrananh.ToString());
            // string dtt1 = matrananh.ToString();

            //string fileName = Application.StartupPath + "\\dtt1.txt";
            //StreamWriter sw = new StreamWriter(fileName, false);
            //sw.WriteLine(dtt1);
            //sw.Close();

            kketnoi.connect();
            SqlCommand cm = new SqlCommand("insert into sinhvien values('" + mssvtxt.Text + "','" + textBox1.Text + "','','','','','" + loptxt.Text + "')", kketnoi.con);
            cm.ExecuteNonQuery();
            SqlCommand cm2 = new SqlCommand("insert into diemdanh values('" + mssvtxt.Text + "','','','','','','','','','','','')", kketnoi.con);
            cm2.ExecuteNonQuery();

            kketnoi.connectClose();

        }

        private void addface_Click(object sender, EventArgs e)
        {

            try
            {
                if (textBox1.Text == "" | loptxt.Text == "" | mssvtxt.Text == "") MessageBox.Show("Chưa nhập đủ thông tin");
                else
                {
                    gray = grabber.QueryGrayFrame().Resize(640, 480, INTER.CV_INTER_CUBIC);

                    //Face Detector
                    MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
             face,
             1.1,
             5,
             Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.FIND_BIGGEST_OBJECT,
             new Size(60, 60));
                    //Action for each element detected
                    foreach (MCvAvgComp f in facesDetected[0])
                    {
                        //  resultface = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                        TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();

                        break;
                    }

                    if (resultface == null) { timer3.Start(); return; }
                    //resize face detected image for force to compare the same size with the 
                    //test image with cubic interpolation type method
                    TrainedFace = resultface.Resize(100, 100, INTER.CV_INTER_CUBIC);
                    //them ten va face vao mang


                    //Show face added in gray scale
                    //if (dem == 0)
                    imageBox1.Image = TrainedFace;


                    try
                    {
                        //TrainedFace.Save(directory + "face" + matrix1s.Count + ".bmp");
                        grabber.QueryFrame().Resize(640, 480, INTER.CV_INTER_CUBIC).Save(directory + textBox1.Text + matrix1s.Count + ".bmp");

                    }
                    catch (Exception ex)
                    {
                        for (int i = matrix1s.Count; i < dem; i++)
                        {
                            // File.Delete(directory + "face" + (matrix1s.Count + dem) + ".bmp");
                            File.Delete(directory + textBox1.Text + (matrix1s.Count + dem) + ".bmp");
                        }
                    }

                    //tface.Add(TrainedFace);
                    TrainedFace = TrainedFace.Resize(50, 50, INTER.CV_INTER_CUBIC);

                    Bitmap tam = TrainedFace.ToBitmap();
                    Bitmap bmnewsize = new Bitmap(tam, newsizegb);
                    x = PCA.image_2_matrix(bmnewsize);
                    x = Radon1.ApdungRadon(x);// PCA.apDungWaveletGabors(x, 0, 1.56, 1);
                    matrix1stam.Add(x);
                    matrix1s.Add(x);
                    labels.Add(mssvtxt.Text);



                    if (dem != 9)
                        addface.Text = "Add face " + (dem + 2).ToString();
                    dem++;

                    if (dem == 10)
                    {
                        luuanh();
                        MessageBox.Show(textBox1.Text + "'s Face detected and added :)", "Training OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dem = 0; mauso = 1;
                        // tface = null; tface = new List<Image<Gray, byte>>();
                        matrix1stam = null; matrix1stam = new List<Matrix1>();
                        x = null;
                        imageBox1.Image = null;
                        ibe1.Image = ibe2.Image = ibn.Image = ibm.Image = null;
                        addface.Text = "Add face 1";
                        resultface = resulteyeL = resulteyeR = resultmouth = resultnose = null;
                        refreshdata();

                    }
                    HDfaces++;
                    mauso++;
                    if (HDfaces <= 10)
                    {

                        label9.Text = mauso.ToString();
                        pictureBox1.Image = Image.FromFile(Application.StartupPath.ToString() + "/huongdan/" + HDfaces.ToString() + ".bmp");
                        //MessageBox.Show(HDfaces.ToString());
                    }

                }

            }
            catch (Exception ex)
            {
                dem = 0;
                MessageBox.Show(ex.ToString(), "Training Fail", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private void NhanDang_Load(object sender, EventArgs e)
        {
            label9.Text = mauso.ToString();
            pictureBox1.Image = Image.FromFile(Application.StartupPath.ToString() + "/huongdan/" + HDfaces.ToString() + ".bmp");
            try
            {
                laydslop();

                refreshdata();
            }
            catch (Exception ex) { MessageBox.Show("Loi ket noi may chu" + ex.ToString()); }
        }
        

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "AVI|*.avi";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = saveFileDialog1.FileName;
                double fpsInitial = grabber.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES);

                int fps = (int)fpsInitial;
                VW = new VideoWriter(path, 4, 640, 480, true);
                //VW.WriteFrame(currentFrame);
                try {
                    //foreach (String h in hex)
                    //{
                    //    Image frame;
                    //    frame = library.ConvertByteToImage(library.DecodeHex(h));

                    //    Image<Gray, Byte> normalizedMasterImage = new Image<Gray, Byte>((Bitmap)frame);
                    //    VW.WriteFrame(normalizedMasterImage);
                        
                    //}
                    //MessageBox.Show("Video Generated Successfully", "Success");
                    luuvideo = true;
                }catch(Exception ei)
                {
                    //VW.Dispose();
                    MessageBox.Show(ei.ToString());
                }
            }
        }
        bool luuvideo=false;
        private void button4_Click(object sender, EventArgs e)
        {
            luuvideo = false;
            VW.Dispose();
        }

        public void laydslop()
        {

            kketnoi.connect();
            dt = kketnoi.laydl("select malop,tenlop from lop");
            loptxt.DataSource = comboBox1.DataSource = dt;
            loptxt.ValueMember = comboBox1.ValueMember = "malop";
            loptxt.DisplayMember = comboBox1.DisplayMember = "malop";
            //       MessageBox.Show(loptxt.Text + loptxt.GetColumnValue("malop"));
            kketnoi.connectClose();
            ktdslop = true;
        }
        public void refreshdata()
        {

            // MessageBox.Show("a");
            mauso = 1;
            HDfaces = 1;
            label9.Text = mauso.ToString();
            pictureBox1.Image = Image.FromFile(Application.StartupPath.ToString() + "/huongdan/" + HDfaces.ToString() + ".bmp");
            string malop = " and sv.malop=" + comboBox1.Text.Trim();
            if (loptxt.Text == "") malop = "";

            gridControl1.DataSource = kketnoi.laydl("select sv.MSSV,TenSV,TenLop,Tuan1,Tuan2,Tuan3,Tuan4,Tuan5,Tuan6,Tuan7,Tuan8,Tuan9,Tuan10,Tong from sinhvien sv,lop l,DiemDanh d where sv.MaLop=l.MaLop and d.mssv=sv.mssv" + malop + "");

            kketnoi.connectClose();

        }




        private void diemdanh()
        {
            try
            {
                if (comboBoxEdit1.Text != "" && name1 != "")
                {
                    string s = kketnoi.lay1dong("select tuan" + comboBoxEdit1.Text + " from sinhvien s, diemdanh d where s.MSSV=d.MSSV and TenSV= '" + name1 + "'"); //co_sua

                    if (s == "x") return;
                    kketnoi.connect();
                    SqlCommand cm = new SqlCommand("update diemdanh set tuan" + comboBoxEdit1.Text + "='x' where MSSV=(select MSSV from Sinhvien where TenSV= '" + name1 + "')", kketnoi.con); //co_sua
                    cm.ExecuteNonQuery();
                    kketnoi.connectClose();
                    refreshdata();
                    //resultface = null;
                    name1 = "";

                }
            }
            catch (Exception) { }
        }
        //reset


        public void NhanDang_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Canclesaveimage();
            TurnOffCam();
        }

        public void TurnOffCam()
        {
            if (ktgrabber)
            {
                grabber.Dispose();
                Application.Idle -= new EventHandler(FrameGrabber);
                detect.Text = "Start";
                ktgrabber = false;
                addface.Enabled = false;
                //cancle.Enabled = false;
            }
        }


        private void diemdanh_Click(object sender, EventArgs e)
        {
            // Canclesaveimage();
            addface.Text = "Add face 1";
        }



        #region xuat file excel
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string malop = "and sv.malop=" + loptxt.Text;
                if (loptxt.Text == "") malop = "";

                DataTable dtx = new DataTable();
                kketnoi.connect();
                dtx = kketnoi.laydl("select sv.MSSV,TenSV,TenLop,Tuan1,Tuan2,Tuan3,Tuan4,Tuan5,Tuan6,Tuan7,Tuan8,Tuan9,Tuan10,Tong from sinhvien sv,lop l,DiemDanh d where sv.MaLop=l.MaLop and d.mssv=sv.mssv " + malop + "");
                kketnoi.connectClose();
                ExportTableToExcel.exportToExcel(dtx, "DanhsachSV.xls");
                MessageBox.Show("OK");
                Process.Start("DanhsachSV.xls");
            }
            catch (Exception) { }
        }
        #endregion

        private void loptxt_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {


                //system.data.row=>return
                if (loptxt.Text.Length > 10)
                    return;
                // MessageBox.Show(loptxt.Text);
                //get all image name  in imagelist

                directorypath = Application.StartupPath + "/Faces/" + loptxt.Text + "";
                if (!Directory.Exists(directorypath))
                {
                    Directory.CreateDirectory(directorypath);
                }
                directory = new DirectoryInfo(directorypath + "/");
                //  Archives = directory.GetFiles("*.bmp");

                loaddanhsachsv();
                refreshdata();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void xoa_Click_1(object sender, EventArgs e)
        {
            try
            {
                ExportTableToExcel.exportToExcel((DataTable)gridControl1.DataSource, "DanhsachSV.xls");
                MessageBox.Show("OK");
                Process.Start("DanhsachSV.xls");
            }
            catch (Exception) { }
        }

        private void comboBoxEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;
        }



        List<Matrix1> matrix1s = new List<Matrix1>();
        List<Matrix1> matrix1stam = new List<Matrix1>();
        Matrix1 x;
        Matrix1 y, matrixtam;
        Size newsizegb = new Size(30, 30);
        string si;

        public void gabor(Bitmap bmg)
        {

            //Bitmap bmg = new Bitmap(filenamepath);
            Bitmap tam = new Bitmap(bmg, newsizegb);
            x = PCA.image_2_matrix(tam);
            x = Radon1.ApdungRadon(x);//PCA.apDungWaveletGabors(x, 0, 1.56, 1);

        }

        private void mssvtxt_Validated(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt16(kketnoi.lay1dong("select count(mssv) from sinhvien where mssv= '" + mssvtxt.Text + "'")) == 1)
                {
                    MessageBox.Show("MSSV không được trùng");
                    return;
                }
            }
            catch (Exception ei) { MessageBox.Show(ei.ToString()); }

        }

    }
}
