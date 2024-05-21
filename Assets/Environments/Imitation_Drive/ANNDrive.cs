using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ANNDrive : MonoBehaviour
{
    DriveANN ann;
    public float visibleDistance=50.0f;
    public int epochs=1000;
    public float speed=50.0f;
    public float rotationSpeed=100.0f;
    bool traningDone = false;
    float trainingProgress=0;
    double sse=0;
    double lastSSE=1;
    public float translation;
    public float rotation;
    public bool loadFromFile=true;

    private string myWeights = "0.46163738281397,-0.176185107519599,0.116979376497766,-0.228423430377974,0.217333362522623,-0.306051568800845,0.493257009266928,-0.0839480764923055,-0.0385918077326093,-0.329487547486579,0.0333295259380069,0.143093781460393,-0.278899774561087,-0.467524326500411,0.322790682843294,-0.0942549486599673,-0.143721281954679,0.381934452666992,0.476973215613022,-0.276893895385326,0.218810143474872,0.414992939331529,-0.248001811500256,-0.494736161811463,-0.22483811252133,-0.0679198491988481,-0.013807188162235,-0.165009862968812,0.219521869531244,-0.320320104588556,-0.424309227788246,0.461757233392974,0.0046523134432051,-0.154105473135569,-0.112265564946153,0.331757016382991,0.14665469782703,-0.315549880766448,-0.114726930386029,0.387105597529434,0.021137670153177,-0.445819940797705,-0.121047873292175,0.434080778810176,-0.297759179707494,-0.158369565303385,-0.0997349216899546,-0.354773631949152,0.371083333752264,-0.279229520796572,0.387384020504155,-0.0880051971087315,-0.333742237869105,0.270064810834177,-0.0738794721574134,0.348907084544379,-0.256389959680583,0.298790099733723,-0.311636551248576,-0.00569830945461017,-0.239424696089773,-0.0402452610075763,-0.10145038042837,-0.0941935495599501,0.150278722784604,0.135687359695816,0.155158050447325,0.160488087677202,-0.0713624315894491,0.0804271504058791,-0.0107246951087293,0.0151062216678336,0.206569853577679,-0.0425501424144458,0.0788495562377335,-0.147762872339939,-0.186288139134042,-0.0401755558202022,-0.135504137103541,-0.151530862908747,-0.0237410853139052,0.0978556317991658,0.103782877322206,0.0566622435912583,0.0168718495697933,0.00101231343926866,0.215460655833153,-0.144752859531797,0.00608385411428899,0.0570524265638745,0.202041331721365,-0.342692399494959,-0.201786138974125,0.057619496781408,-0.0573487020919699,0.0719271385659367,0.180647921006143,0.240950363225353,-5.58386477824987E-05,-0.059127113095836,-0.0492678580596345,0.170815904583278,-0.304215308241804,-0.22732407672052,-0.0768022039806464,-0.210906459927236,0.0851369941033309,0.0510678590972826,-0.110231470418254,-0.306691787546752,-0.0629145776198185,-0.120247800865332,-0.138462487054363,0.141947026563488,-0.270168459861638,0.116338164915271,0.145287660299891,-0.156841790348086,0.130668373525682,-0.0381504270868169,0.179693835333452,-0.0843365213692706,0.0293090909310655,-0.0999053781971461,-0.0435731052621227,0.116446439502464,0.310133871958991,0.317040130744579,-0.146355772775583,0.127905800959338,0.359982406583881,-0.0103328755084544,-0.238303133098837,-0.236920771293981,-0.179902249279048,-0.188138173652369,-0.0899507166228221,-0.164551730568111,-0.03062853009749,-0.1105793155204,-0.173192093319028,-0.100099385750336,0.106849259010543,-0.0776379799063918,0.0526414453670735,-0.16105390342114,0.0608349241308451,-0.225400260503936,0.0965618555719412,-0.0326984397136509,0.11604186157756,0.0519660633257496,-0.252210478266726,0.14797488004677,0.157309389665639,0.0643022068940381,-0.112965993686518,0.0513416350398363,-0.156782047919645,-0.0319781633153101,-0.199599282930255,-0.0676176357760171,0.185359583694866,0.142922413630761,0.203964901886724,-0.0139445761419825,-0.000170829046413145,0.161215686022613,-0.233533660025355,-0.0844801537708034,1.15942532989368,0.388796825425472,2.46989232236564,-0.251999218987451,0.29494913794329,0.531361295546851,0.842977207268873,0.30385101948721,0.302586172170992,0.39419375452475,-0.888229003542123,1.01252013025841,0.208813128091538,2.17950331863791,1.74602229697414,0.7970374908091,0.592744931257141,2.35004495380758,0.85611007660137,0.293834715929193,1.14180416999612,1.79092313553567,";

    void Start(){
        ann = new DriveANN(5,2,2,10,0.5);
        ann.LoadWeights(myWeights);
        traningDone=true;
        // if(loadFromFile){
        //     LoadWeightsFromFile();
        // } else {
        //     StartCoroutine(LoadTrainingSet());
        // }
    }

    void OnGUI(){
        GUI.Label(new Rect(25,25,250,100),"SSE: "+lastSSE);
        GUI.Label(new Rect(25,40,250,100),"Alpha: "+ann.alpha);
        GUI.Label(new Rect(25,55,250,100),"Trained: "+trainingProgress);
    }

    // IEnumerator LoadTrainingSet(){
    //     string path = Application.dataPath + "/Environments/Imitation_Drive/trainingData.txt";
    //     string line;
    //     if(File.Exists(path)){
    //         int lineCount = File.ReadAllLines(path).Length;
    //         StreamReader tdf = File.OpenText(path);
    //         List<double> calcOutputs = new List<double>();
    //         List<double> inputs = new List<double>();
    //         List<double> outputs = new List<double>();

    //         for(int i=0;i<epochs;i++){
    //             sse=0;
    //             tdf.BaseStream.Position=0;
    //             string currentWeights = ann.PrintWeights();
    //             while((line=tdf.ReadLine())!=null){
    //                 string[] data = line.Split(",");
    //                 float thisError=0;
    //                 if(System.Convert.ToDouble(data[5])!=0 && System.Convert.ToDouble(data[6])!=0){
    //                     inputs.Clear();
    //                     outputs.Clear();
    //                     inputs.Add(System.Convert.ToDouble(data[0]));
    //                     inputs.Add(System.Convert.ToDouble(data[1]));
    //                     inputs.Add(System.Convert.ToDouble(data[2]));
    //                     inputs.Add(System.Convert.ToDouble(data[3]));
    //                     inputs.Add(System.Convert.ToDouble(data[4]));

    //                     double o1=Map(0,1,-1,1,System.Convert.ToSingle(data[5]));
    //                     outputs.Add(o1);
    //                     double o2=Map(0,1,-1,1,System.Convert.ToSingle(data[6]));
    //                     outputs.Add(o2);

    //                     calcOutputs = ann.Train(inputs,outputs);
    //                     thisError = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2) + Mathf.Pow((float)(outputs[1] - calcOutputs[1]),2))/2.0f;
    //                 }
    //                 sse+=thisError;
    //             }
    //             trainingProgress=(float)i/(float)epochs;
    //             sse/=lineCount;
    //             // lastSSE=sse;

    //             if(lastSSE < sse){
    //                 ann.LoadWeights(currentWeights);
    //                 ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f,0.01f,0.9f);
    //             } else {
    //                 ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f,0.01f,0.9f);
    //                 lastSSE = sse;
    //             }

    //             yield return null;
    //         }
    //     }
    //     traningDone=true;
    //     SaveWeightsToFile();
    // }

    // void SaveWeightsToFile(){
    //     string path = Application.dataPath + "/Environments/Imitation_Drive/weights.txt";
    //     // Debug.Log(path);
    //     StreamWriter wf = File.CreateText(path);
    //     wf.WriteLine(ann.PrintWeights());
    //     wf.Close();
    // }

    // void LoadWeightsFromFile(){
    //     // string path = Application.dataPath + "/Environments/Imitation_Drive/weights.txt";
    //     // StreamReader wf = File.OpenText(path);
    //     // if(File.Exists(path)){
    //     //     string line = wf.ReadLine();
    //     //     ann.LoadWeights(line);
    //     // }
    //     ann.LoadWeights(myWeights);
    // }

    float Map(float newfrom,float newto,float origfrom, float origto,float value){
        if(value<=origfrom)return newfrom;
        else if(value>=origto) return newto;
        return (newto - newfrom) * ((value - origfrom)/(origto - origfrom)) + newfrom;
    }

    float Round(float x){
        return (float) System.Math.Round(x,System.MidpointRounding.AwayFromZero);
    }

    public void GetData(List<double>inputs, List<double> outputs){
        List<double> calcOutputs;
        calcOutputs = ann.Train(inputs,outputs);
        float err = (Mathf.Pow((float)(outputs[0] - calcOutputs[0]),2) + Mathf.Pow((float)(outputs[1] - calcOutputs[1]),2))/2.0f;
        if(err<lastSSE){
            lastSSE=err;
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Loader.Load(Loader.Scene.MainMenuScene);
        }
        if(!traningDone) return;
        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        Debug.DrawRay(transform.position,this.transform.forward * visibleDistance, Color.red);

        Debug.DrawRay(transform.position,this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position,-this.transform.right * visibleDistance, Color.red);

        Debug.DrawRay(transform.position,Quaternion.AngleAxis(-45,Vector3.up) * this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position,Quaternion.AngleAxis(45,Vector3.up) * -this.transform.right * visibleDistance, Color.red);

        RaycastHit hit;
        float fdist = 0, rdist = 0, ldist=0, r45dist=0, l45dist=0;

        if(Physics.Raycast(transform.position,this.transform.forward,out hit, visibleDistance)){
            fdist =1 - Round(hit.distance/visibleDistance);
        }

        if(Physics.Raycast(transform.position,this.transform.right,out hit, visibleDistance)){
            rdist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position, -this.transform.right,out hit, visibleDistance)){
            ldist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position,Quaternion.AngleAxis(-45,Vector3.up) * this.transform.right,out hit, visibleDistance)){
            r45dist =1 - Round(hit.distance/visibleDistance);
        }
        
        if(Physics.Raycast(transform.position,Quaternion.AngleAxis(45,Vector3.up) * -this.transform.right,out hit, visibleDistance)){
            l45dist =1 - Round(hit.distance/visibleDistance);
        }

        inputs.Add(fdist);
        inputs.Add(rdist);
        inputs.Add(ldist);
        inputs.Add(r45dist);
        inputs.Add(l45dist);

        outputs.Add(0);
        outputs.Add(0);
        calcOutputs = ann.CalcOutput(inputs,outputs);
        float translationInput = Map(-1,1,0,1,(float)calcOutputs[0]);
        float rotationInput = Map(-1,1,0,1,(float)calcOutputs[1]);

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation =translationInput * speed * Time.deltaTime;
        rotation =rotationInput * rotationSpeed * Time.deltaTime;

        // Move translation along the object's z-axis
        this.transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        this.transform.Rotate(0, rotation, 0);
    }
}
