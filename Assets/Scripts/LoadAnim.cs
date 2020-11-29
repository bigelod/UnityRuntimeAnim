using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadAnim : MonoBehaviour
{
    public CustAnimator myAnim = new CustAnimator();

    [SerializeField]
    private string fileName = "SampleAnim.txt";
    [SerializeField]
    private bool parseData = true;

    [SerializeField]
    private bool debugData = false;
    [SerializeField]
    private List<string> variableData = new List<string>();

    // Update is called once per frame
    void Update()
    {
        if (parseData)
        {
            if (myAnim != null) myAnim.clearData();

            myAnim = new CustAnimator(transform);

            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            loadFile(filePath);

            parseData = false;
        }

        if (myAnim.isLoaded) myAnim.update();
    }

    private void OnGUI()
    {
        if (debugData)
        {
            string debugTxt = "";

            foreach (string str in variableData)
            {
                if (debugTxt != "") debugTxt += System.Environment.NewLine;
                debugTxt += str + ": " + myAnim.getVariable(str);
            }

            GUILayout.Label(debugTxt);
        }
    }

    //Load a file to memory
    private void loadFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            List<string> data = new List<string>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    data.Add(sr.ReadLine());
                }
            }

            if (data.Count > 0) parseLines(data);
        }
    }

    //Parse the data from file into animation data
    private void parseLines(List<string> data)
    {
        foreach (string line in data)
        {
            string[] split = line.Split(',');

            if (split.Length > 12)
            {
                //Key frame
                KeyFrame frame = new KeyFrame();

                if (split.Length > 13)
                {
                    //For a new object
                    int id = 0;
                    int.TryParse(split[0], out id);
                    frame.objID = id;

                    frame.objDesc = split[1];
                    frame.animName = split[2];

                    int childOf = 0;
                    int.TryParse(split[3], out childOf);
                    frame.childOf = childOf;

                    int frameTime = 0;
                    int.TryParse(split[4], out frameTime);
                    frame.frameTime = frameTime;

                    int instant = 0;
                    int.TryParse(split[5], out instant);
                    if (instant > 0) frame.instant = true;

                    float pX = 0f;
                    float pY = 0f;
                    float pZ = 0f;

                    float.TryParse(split[6], out pX);
                    float.TryParse(split[7], out pY);
                    float.TryParse(split[8], out pZ);

                    frame.position = new Vector3(pX, pY, pZ);

                    float rX = 0f;
                    float rY = 0f;
                    float rZ = 0f;

                    float.TryParse(split[9], out rX);
                    float.TryParse(split[10], out rY);
                    float.TryParse(split[11], out rZ);

                    frame.rotation = new Vector3(rX, rY, rZ);

                    float sX = 0f;
                    float sY = 0f;
                    float sZ = 0f;

                    float.TryParse(split[12], out sX);
                    float.TryParse(split[13], out sY);
                    float.TryParse(split[14], out sZ);

                    frame.scale = new Vector3(sX, sY, sZ);

                    myAnim.AddKeyFrame(frame.animName, frame);
                }
                else
                {
                    //For an existing object
                    int id = 0;
                    int.TryParse(split[0], out id);
                    frame.objID = id;

                    frame.animName = split[1];

                    int frameTime = 0;
                    int.TryParse(split[2], out frameTime);
                    frame.frameTime = frameTime;

                    int instant = 0;
                    int.TryParse(split[3], out instant);
                    if (instant > 0) frame.instant = true;

                    float pX = 0f;
                    float pY = 0f;
                    float pZ = 0f;

                    float.TryParse(split[4], out pX);
                    float.TryParse(split[5], out pY);
                    float.TryParse(split[6], out pZ);

                    frame.position = new Vector3(pX, pY, pZ);

                    float rX = 0f;
                    float rY = 0f;
                    float rZ = 0f;

                    float.TryParse(split[7], out rX);
                    float.TryParse(split[8], out rY);
                    float.TryParse(split[9], out rZ);

                    frame.rotation = new Vector3(rX, rY, rZ);

                    float sX = 0f;
                    float sY = 0f;
                    float sZ = 0f;

                    float.TryParse(split[10], out sX);
                    float.TryParse(split[11], out sY);
                    float.TryParse(split[12], out sZ);

                    frame.scale = new Vector3(sX, sY, sZ);

                    myAnim.AddKeyFrame(frame.animName, frame);
                }
            }
            else if (split.Length > 4)
            {
                //Variable frame
                VarFrame frame = new VarFrame();
                frame.varName = split[0];
                frame.animName = split[1];

                int frameTime = 0;
                int instant = 0;
                float value = 0f;

                int.TryParse(split[2], out frameTime);
                frame.frameTime = frameTime;

                int.TryParse(split[3], out instant);
                if (instant > 0) frame.instant = true;

                float.TryParse(split[4], out value);
                frame.value = value;

                myAnim.AddVarFrame(split[1], frame);
            }
            else if (split.Length > 2)
            {
                //Animation configuration
                string animName = split[0];

                int loopVal = 0;
                int.TryParse(split[1], out loopVal);
                bool loop = false;
                if (loopVal > 0) loop = true;

                string goToAnim = split[2];

                if (goToAnim.ToUpper() == "NA") goToAnim = "";

                myAnim.AnimConfig(animName, loop, goToAnim);
            }
        }

        myAnim.isLoaded = true; //Done loading
    }
}

[System.Serializable]
public class KeyFrame
{
    public int objID; //The unique ID of the object, used for hierarchy
    public string objDesc; //Object descriptor or "slot name" for the attached asset
    public string animName; //The unique animation name
    public int childOf; //The ID we are a child of, if greater than 0 then all positions, rotations, and scales are relative
    public int frameTime; //The frame time in ms
    public bool instant; //Linear interpolation up to the frame or instant change after this frame is hit?
    public Vector3 position; //Position of this object
    public Vector3 rotation; //Rotation of this object
    public Vector3 scale; //Scale of this object
    public bool creationFrame; //Was this the first frame we see the object?

    public KeyFrame()
    {
        objID = -1;
        objDesc = "";
        animName = "";
        childOf = 0;
        frameTime = 0;
        instant = false;
        position = new Vector3(0f, 0f, 0f);
        rotation = new Vector3(0f, 0f, 0f);
        scale = new Vector3(0f, 0f, 0f);
        creationFrame = false;
    }
}

[System.Serializable]
public class VarFrame
{
    public string varName; //Unique name of the variable
    public string animName; //Unique animation name
    public int frameTime; //The time in the animation in ms
    public bool instant; //Smoothly interpolate between values (decimals) or instant change?
    public float value; //The value to animate
    public bool creationFrame; //Is this a creation frame?

    public VarFrame()
    {
        varName = "";
        animName = "";
        frameTime = 0;
        instant = false;
        value = 0f;
        creationFrame = false;
    }
}

[System.Serializable]
public class CustAnim
{
    public string name;
    public int duration; //Animation duration
    public bool loop; //Whether to loop the animation or not
    public string goToAnim; //Which animation to go to when this ends
    public List<KeyFrame> keyFrames; //The animation keyframes
    public List<VarFrame> varFrames; //The variable keyframes
    public List<int> objs; //The list of unique object IDs
    public List<string> objDescs; //Object descriptions
    public Dictionary<int, GameObject> assignedObjects; //The objects assigned to each object slot / ID
    public Dictionary<string, float> variables; //The variables in the animation

    public CustAnim()
    {
        name = "";
        duration = 0;
        loop = false;
        goToAnim = "";
        keyFrames = new List<KeyFrame>();
        varFrames = new List<VarFrame>();
        objs = new List<int>();
        objDescs = new List<string>();
        variables = new Dictionary<string, float>();
        assignedObjects = new Dictionary<int, GameObject>();
    }

    //Release objects
    public void releaseObjects()
    {
        foreach (KeyValuePair<int, GameObject> pair in assignedObjects)
        {
            pair.Value.transform.parent = null; //Release the parenting information
        }
    }
}

[System.Serializable]
public class CustAnimator
{
    public string currentAnim;
    public List<CustAnim> animations; //List of animations
    public float animSpeed; //Animator speed
    public bool isPlaying; //Are we playing
    public float currTime; //Current time in milliseconds
    public bool isLoaded; //Have we loaded?
    public Transform baseTransform; //What object is holding us?

    public CustAnimator()
    {
        currentAnim = "Default";
        animations = new List<CustAnim>();
        animSpeed = 1f;
        isPlaying = true;
        currTime = 0f;
        isLoaded = false;
        baseTransform = null;
    }

    public CustAnimator(Transform t)
    {
        currentAnim = "Default";
        animations = new List<CustAnim>();
        animSpeed = 1f;
        isPlaying = true;
        currTime = 0f;
        isLoaded = false;
        baseTransform = t;
    }

    //Restart the animation from the beginning
    public void rewind()
    {
        currTime = 0f;
    }

    //Play the animation
    public void play()
    {
        isPlaying = true;
    }

    //Stop the animation
    public void stop()
    {
        isPlaying = false;
    }

    //Get the duration of the animation in ms
    public int duration()
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                return anim.duration;
            }
        }

        //Debug.Log("Animation: " + currentAnim + " does not exist!");
        return 0;
    }

    //Set the animation to loop
    public void setLoop(bool _tf)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                anim.loop = _tf;
                break;
            }
        }
    }

    //Get if the animation is looping
    public bool looping()
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                return anim.loop;
            }
        }

        return false;
    }

    //Set the current time of the animation in ms
    public void setTime(float time)
    {
        currTime = time;
    }

    //Get the value of a variable in an animation
    public float getVariable(string name)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                if (anim.variables.ContainsKey(name))
                {
                    return anim.variables[name];
                }
            }
        }

        //Debug.Log("Variable: " + name + " does not exist in " + currentAnim);
        return 0f;
    }

    //Update all code to do with the animation itself
    public void update()
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                //This is our animation
                foreach (KeyValuePair<int, GameObject> pair in anim.assignedObjects)
                {
                    //Animate each object
                    KeyFrame prev = getPreviousFrame(pair.Value, pair.Key);
                    KeyFrame next = getNextFrame(pair.Value, pair.Key);

                    if (prev.animName != "NA" && next.animName != "NA")
                    {
                        //We can only animate if the frames exist

                        if (prev.creationFrame)
                        {
                            if (prev.childOf > 0)
                            {
                                if (anim.assignedObjects.ContainsKey(prev.childOf))
                                {
                                    //Also make sure we are parented properly
                                    pair.Value.transform.parent = anim.assignedObjects[prev.childOf].transform;
                                }
                            }

                            //Set the base value to start
                            pair.Value.transform.localPosition = prev.position;
                            pair.Value.transform.localEulerAngles = prev.rotation;
                            pair.Value.transform.localScale = prev.scale;
                        }

                        if (next.instant)
                        {
                            //We don't interpolate, we just stay at the previous frame values
                            pair.Value.transform.localPosition = prev.position;
                            pair.Value.transform.localEulerAngles = prev.rotation;
                            pair.Value.transform.localScale = prev.scale;
                        }
                        else
                        {
                            //Smooth interpolation between previous and next frame
                            float percentage = (currTime - prev.frameTime) / (next.frameTime - prev.frameTime);
                            if (float.IsNaN(percentage)) percentage = 0f;

                            pair.Value.transform.localPosition = Vector3.Lerp(prev.position, next.position, percentage);
                            pair.Value.transform.localEulerAngles = Vector3.Lerp(prev.rotation, next.rotation, percentage);
                            pair.Value.transform.localScale = Vector3.Lerp(prev.scale, next.scale, percentage);
                        }
                    }
                }

                //Animate our variables
                List<string> modVars = new List<string>();
                List<float> valVars = new List<float>();

                foreach (KeyValuePair<string, float> pair in anim.variables)
                {
                    VarFrame prev = getPreviousFrame(pair.Key);
                    VarFrame next = getNextFrame(pair.Key);

                    if (prev.animName != "NA" && next.animName != "NA")
                    {
                        if (next.instant)
                        {
                            //Stay at previous value
                            modVars.Add(pair.Key);
                            valVars.Add(prev.value);
                        }
                        else
                        {
                            //Smooth interpolation between previous and next frame
                            float percentage = (currTime - prev.frameTime) / (next.frameTime - prev.frameTime);
                            if (float.IsNaN(percentage)) percentage = 0f;

                            modVars.Add(pair.Key);
                            valVars.Add(Mathf.Lerp(prev.value, next.value, percentage));
                        }
                    }
                }

                for (int i = 0; i < modVars.Count; i++)
                {
                    anim.variables[modVars[i]] = valVars[i];
                }

                //Timing and transitions
                if (isPlaying)
                {
                    //Advance time and update
                    currTime += 1000f * animSpeed * Time.deltaTime;
                }

                if (currTime >= anim.duration)
                {
                    if (anim.loop)
                    {
                        currTime = 0f; //Restart the animation
                    }
                    else if (anim.goToAnim != "")
                    {
                        setAnim(anim.goToAnim); //Set the animation we are going to
                    }
                    else
                    {
                        isPlaying = false; //Done playing
                        currTime = anim.duration; //Set to end frame
                    }
                }

                break;
            }
        }
    }

    //Set the current animation playing
    public void setAnim(string name)
    {
        bool foundAnim = false;

        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == name.ToUpper())
            {
                anim.releaseObjects();
                currentAnim = anim.name;
                currTime = 0f;
                foundAnim = true;
                break;
            }
        }

        if (!foundAnim) Debug.Log("No animation: " + name + " found!");
    }

    //Add a keyframe to the animation
    public void AddKeyFrame(string animName, KeyFrame frame)
    {
        bool foundAnim = false;

        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == animName.ToUpper())
            {
                if (!anim.objs.Contains(frame.objID))
                {
                    //Creation frame!
                    frame.creationFrame = true;
                    anim.objs.Add(frame.objID);
                    anim.objDescs.Add(frame.objDesc);
                }

                if (anim.duration < frame.frameTime) anim.duration = frame.frameTime; //Increase duration value
                anim.keyFrames.Add(frame);

                foundAnim = true;
                break;
            }
        }

        if (!foundAnim)
        {
            CustAnim anim = new CustAnim();
            anim.name = animName;

            if (!anim.objs.Contains(frame.objID))
            {
                //Creation frame!
                frame.creationFrame = true;
                anim.objs.Add(frame.objID);
                anim.objDescs.Add(frame.objDesc);
            }

            anim.keyFrames.Add(frame);

            animations.Add(anim);
        }
    }

    //Add a variable keyframe to the animation
    public void AddVarFrame(string animName, VarFrame frame)
    {
        bool foundAnim = false;

        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == animName.ToUpper())
            {
                if (!anim.variables.ContainsKey(frame.varName))
                {
                    //Creation frame!
                    frame.creationFrame = true;
                    anim.variables.Add(frame.varName, frame.value);
                }

                anim.varFrames.Add(frame);

                foundAnim = true;
                break;
            }
        }

        if (!foundAnim)
        {
            CustAnim anim = new CustAnim();
            anim.name = animName;

            if (!anim.variables.ContainsKey(frame.varName))
            {
                //Creation frame!
                frame.creationFrame = true;
                anim.variables.Add(frame.varName, frame.value);
            }

            anim.varFrames.Add(frame);

            animations.Add(anim);
        }
    }

    //Configure the animation, usually set from loading a file
    public void AnimConfig(string animationName, bool loop, string GoToAnim = "")
    {
        bool foundAnim = false;

        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == animationName.ToUpper())
            {
                anim.loop = loop;
                anim.goToAnim = GoToAnim;
                foundAnim = true;
                break;
            }
        }

        if (!foundAnim)
        {
            CustAnim anim = new CustAnim();

            anim.name = animationName;
            anim.loop = loop;
            anim.goToAnim = GoToAnim;

            animations.Add(anim);
        }
    }

    //Assign a gameobject with optional value of ID number to assign to
    public void AssignObj(GameObject obj, int ID = 0)
    {
        //Assign an object to a slot if it exists in the current animation
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                if (anim.objs.Contains(ID))
                {
                    if (!anim.assignedObjects.ContainsKey(ID))
                    {
                        anim.assignedObjects.Add(ID, obj);
                        obj.transform.parent = baseTransform;
                    }
                    else
                    {
                        anim.assignedObjects[ID] = obj;
                        obj.transform.parent = baseTransform;
                    }
                }
                break;
            }
        }
    }

    //Get the ID we should be a child of
    public int ChildOf(int ID = 0)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (KeyFrame frame in anim.keyFrames)
                {
                    if (frame.creationFrame && frame.objID == ID)
                    {
                        return frame.childOf;
                    }
                }

                break;
            }
        }

        return 0;
    }

    //Get the slot ID number for an object
    public int SlotID(GameObject obj)
    {
        //Get the name of the slot for this object
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (KeyValuePair<int, GameObject> pair in anim.assignedObjects)
                {
                    if (pair.Value.GetInstanceID() == obj.GetInstanceID())
                    {
                        return pair.Key;
                    }
                }

                break;
            }
        }

        return 0;
    }

    //Get the slot name or description for an object
    public string SlotName(GameObject obj)
    {
        //Get the name of the slot for this object
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (KeyValuePair<int, GameObject> pair in anim.assignedObjects)
                {
                    if (pair.Value.GetInstanceID() == obj.GetInstanceID())
                    {
                        int slotID = pair.Key;
                        return anim.objDescs[slotID];
                    }
                }

                break;
            }
        }

        return "";
    }

    //Get the next keyframe for an object
    public KeyFrame getNextFrame(GameObject obj, int slotID = -1)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (KeyFrame frame in anim.keyFrames)
                {
                    if (slotID == -1) slotID = SlotID(obj);
                    if (frame.objID == slotID)
                    {
                        //This frame has to be for us
                        if (frame.frameTime >= currTime)
                        {
                            return frame;
                        }
                    }
                }

                break;
            }
        }

        //Debug.Log("Animation " + currentAnim + " not found or animation does not contain object " + obj.name);
        KeyFrame newFrame = new KeyFrame();

        newFrame.animName = "NA";

        return newFrame;
    }

    //Get the previous keyframe for an object
    public KeyFrame getPreviousFrame(GameObject obj, int slotID = -1)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                KeyFrame prevFrame = new KeyFrame();

                foreach (KeyFrame frame in anim.keyFrames)
                {
                    if (slotID == -1) slotID = SlotID(obj);
                    if (frame.objID == slotID)
                    {
                        //This frame has to be for us
                        if (frame.frameTime <= currTime)
                        {
                            if ((frame.creationFrame && currTime <= 32f) || currTime >= anim.duration)
                            {
                                //This is the first frame for this object, likely a starting time, or otherwise the last frame where we "stay" unless we loop
                                return frame;
                            }
                            else
                            {
                                //Store this one for later, for now we can assume the file is in chronological order until we have a better JSON or XML format
                                prevFrame = frame;
                            }
                        }
                        else
                        {
                            //We went too far now, return the previous one
                            return prevFrame;
                        }
                    }
                }

                break;
            }
        }

        //Debug.Log("Animation " + currentAnim + " not found or animation does not contain object " + obj.name);
        KeyFrame newFrame = new KeyFrame();

        newFrame.animName = "NA";

        return newFrame;
    }

    //Get the next variable frame
    public VarFrame getNextFrame(string varName)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (VarFrame frame in anim.varFrames)
                {
                    if (frame.varName == varName)
                    {
                        //This frame has to be for us
                        if (frame.frameTime >= currTime)
                        {
                            return frame;
                        }
                    }
                }

                break;
            }
        }

        //Debug.Log("Animation " + currentAnim + " not found or animation does not contain variable " + varName);
        VarFrame newFrame = new VarFrame();

        newFrame.animName = "NA";

        return newFrame;
    }

    //Get the previous variable frame
    public VarFrame getPreviousFrame(string varName)
    {
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                VarFrame prevFrame = new VarFrame();

                foreach (VarFrame frame in anim.varFrames)
                {
                    if (frame.varName == varName)
                    {
                        //This frame has to be for us
                        if (frame.frameTime <= currTime)
                        {
                            if ((frame.creationFrame && currTime < 32f) || currTime >= anim.duration)
                            {
                                //This is within the first frame of existence, likely a starting time, or otherwise the last frame where we "stay" unless we loop
                                return frame;
                            }
                            else
                            {
                                //Store this one for later, for now we can assume the file is in chronological order until we have a better JSON or XML format
                                prevFrame = frame;
                            }
                        }
                        else
                        {
                            //We went too far now, return the previous one
                            return prevFrame;
                        }
                    }
                }

                break;
            }
        }

        //Debug.Log("Animation " + currentAnim + " not found or animation does not contain variable " + varName);
        VarFrame newFrame = new VarFrame();

        newFrame.animName = "NA";

        return newFrame;
    }

    //List the animations in this player in a CSV
    public string listAnimations()
    {
        string ans = "";
        bool addComma = false;
        foreach (CustAnim anim in animations)
        {
            if (addComma) ans += ",";
            ans += anim.name;
            addComma = true;
        }

        return ans;
    }

    //List the objects in the current animation
    public string listObjects()
    {
        string ans = "";
        bool addComma = false;
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (string desc in anim.objDescs)
                {
                    if (addComma) ans += ",";
                    ans += desc;
                    addComma = true;
                }
            }
        }

        return ans;
    }

    //List the variables in the current animation
    public string listVariables()
    {
        string ans = "";
        bool addComma = false;
        foreach (CustAnim anim in animations)
        {
            if (anim.name.ToUpper() == currentAnim.ToUpper())
            {
                foreach (KeyValuePair<string, float> pair in anim.variables)
                {
                    if (addComma) ans += ",";
                    ans += pair.Key;
                    addComma = true;
                }
            }
        }

        return ans;
    }

    //Clear the animation data (eg: destroying the animator or reloading data)
    public void clearData()
    {
        isLoaded = false;

        foreach (CustAnim anim in animations)
        {
            anim.releaseObjects();
        }
    }
}