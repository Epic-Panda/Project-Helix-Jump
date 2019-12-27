using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlling design of every level.
/// </summary>
public class SpawnController : MonoBehaviour
{
    [Header("Ball pref")]
    public GameObject ballPref;

    [Header("Platform prefs")]
    public GameObject fullCircle;
    public GameObject circle_1of20;
    public GameObject circle_2of20;
    public GameObject circle_5of20;
    public GameObject circle_12of20;
    public GameObject circle_17of20;
    public GameObject circle_1of20_escapeTrigger;
    public GameObject circle_2of20_escapeTrigger;
    public GameObject circle_5of20_escapeTrigger;
    public GameObject smallMovingDestroyPlatform;

    [Header("Platform color")]
    public Color pilarColor;
    public Color ballColor;
    public Color safeColor;
    public Color destroyColor;
    public Color backgroundColor;

    [Header("Material")]
    public Material safePlatform;
    public Material destroyPlatform;
    public Material cleanHitPlatformMaterial;
    public Material backgroundMaterial;
    public Material ballMaterial;
    public Material pillarMaterial;

    [Header("Platform piece")]
    public int maxNumberOfPlatformPieces = 20;
    public int minNumOfEscapePlatformPieces = 2;
    public GameObject platformsContainer;

    [Header("Platform")]
    public float startYPosition;
    public int platformNumber;
    public float platformDistance;

    float yRotationPerOneSlice = 18;
    GameObject ball;
    Vector3 ballPosition;

    /// <summary>
    /// Start function is calling functions for platform creation and then sets up colors.
    /// </summary>
    void Start()
    {
        CreatePlatforms();
        ball = Instantiate(ballPref);
        ballPosition = ball.transform.position;
        ball.GetComponent<BallController>().breakWorth = GameManager.instance.level;

        if (PlayerPrefs.GetInt("level") == 1)
            SetCollors();
        else
            RandomizeCollors();
    }

    /// <summary>
    /// Creating platforms.
    /// </summary>
    void CreatePlatforms()
    {
        for (int i = 0; i < platformNumber; i++)
        {
            GameObject platformPieceContainer = new GameObject("Piece Container");
            platformPieceContainer.transform.parent = platformsContainer.transform;
            platformPieceContainer.tag = "piece container";

            int numberOfSafePlatforms = 0;
            int numberOfDestroyPlatforms = 0;
            int numberOfEscapes = 0;

            if (i != 0 && i != platformNumber - 1)
            {

                numberOfSafePlatforms = Random.Range(8, maxNumberOfPlatformPieces - minNumOfEscapePlatformPieces);
                numberOfDestroyPlatforms = Random.Range(0, maxNumberOfPlatformPieces - numberOfSafePlatforms - minNumOfEscapePlatformPieces);
                numberOfEscapes = maxNumberOfPlatformPieces - numberOfSafePlatforms - numberOfDestroyPlatforms;

                if (numberOfEscapes % 2 != 0)
                {
                    if (numberOfSafePlatforms > 8 || numberOfDestroyPlatforms == 0)
                        numberOfSafePlatforms--;
                    else
                        numberOfDestroyPlatforms--;

                    numberOfEscapes++;
                }
            }
            else if (i == 0)
            {
                numberOfEscapes = Random.Range(2, 8);
                if (numberOfEscapes % 2 != 0)
                    numberOfEscapes++;

                numberOfSafePlatforms = maxNumberOfPlatformPieces - numberOfEscapes;
            }
            else
            {
                GameObject slice = Instantiate(fullCircle, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                slice.tag = "undestructable";
                return;
            }

            List<int> slices = new List<int>();

            for (int j = 0; j < numberOfSafePlatforms; j++)
            {
                slices.Add(1);
            }

            for (int j = 0; j < numberOfDestroyPlatforms; j++)
            {
                slices.Add(2);
            }

            for (int j = 0; j < numberOfEscapes; j++)
            {
                slices.Add(3);
            }

            float angle = 0;

            int type = 0;
            int sameTypeCount = 0;

            while (slices.Count > 0)
            {
                int index = 0;

                if (slices.Count > 1)
                    index = Random.Range(0, slices.Count - 1);

                int rand = slices[index];

                if (type == 0)
                    type = rand;

                // to make sure there is even number of escape platform pieces
                if (type == 3 && rand != 3 && sameTypeCount % 2 != 0)
                {
                    index = slices.Count - 1;
                    rand = slices[index];
                }

                slices.RemoveAt(index);

                // if type is same just keep counting
                if (type == rand)
                {
                    sameTypeCount++;

                    // if there is more than 0 slices keep returning and counting
                    if (slices.Count > 0)
                        continue;
                }

                GameObject slice = null;

                // continue from here, used for last piece
                whileStart:

                int movingDestroyPieceSize = 0;
                if (type == 3)
                    movingDestroyPieceSize = sameTypeCount - 1;

                while (sameTypeCount > 0)
                {
                    if (sameTypeCount >= 17 && type != 3)
                    {
                        slice = Instantiate(circle_17of20, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                        SetSliceTagAndMaterial(slice, type);

                        if (i > 0)
                            CreateMovingDestroyPlatform(Random.Range(-0.5f, -yRotationPerOneSlice * 7.5f), Random.Range(-yRotationPerOneSlice * 9.5f, -yRotationPerOneSlice * 16), slice);

                        angle += yRotationPerOneSlice * 17;
                        slice.transform.Rotate(new Vector3(0, angle, 0));

                        sameTypeCount -= 17;
                        continue;
                    }

                    if (sameTypeCount >= 12 && type != 3)
                    {
                        slice = Instantiate(circle_12of20, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                        SetSliceTagAndMaterial(slice, type);

                        if (i > 0)
                            CreateMovingDestroyPlatform(Random.Range(-0.5f, -yRotationPerOneSlice * 5), Random.Range(-yRotationPerOneSlice * 7, -yRotationPerOneSlice * 11), slice);

                        angle += yRotationPerOneSlice * 12;
                        slice.transform.Rotate(new Vector3(0, angle, 0));

                        sameTypeCount -= 12;
                        continue;
                    }

                    if (sameTypeCount >= 5)
                    {
                        if (type != 3)
                            slice = Instantiate(circle_5of20, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                        else
                            slice = Instantiate(circle_5of20_escapeTrigger, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);


                        SetSliceTagAndMaterial(slice, type);

                        angle += yRotationPerOneSlice * 5;
                        slice.transform.Rotate(new Vector3(0, angle, 0));

                        if (i > 0 && type != 3)
                            CreateMovingDestroyPlatform(Random.Range(-0.5f, -yRotationPerOneSlice * 1.5f), Random.Range(-yRotationPerOneSlice * 3.5f, -yRotationPerOneSlice * 4), slice);
                        else if (i > 0 && type == 3 && sameTypeCount == 5)
                            CreateMovingDestroyPlatform(-0.5f, -yRotationPerOneSlice * movingDestroyPieceSize, slice);

                        sameTypeCount -= 5;
                        continue;
                    }

                    if (sameTypeCount >= 2)
                    {
                        if (type != 3)
                            slice = Instantiate(circle_2of20, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                        else
                            slice = Instantiate(circle_2of20_escapeTrigger, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);

                        SetSliceTagAndMaterial(slice, type);

                        if (i > 0 && type == 3 && sameTypeCount == 2 && movingDestroyPieceSize >= 3)
                            CreateMovingDestroyPlatform(-0.5f, -yRotationPerOneSlice * movingDestroyPieceSize, slice);

                        angle += yRotationPerOneSlice * 2;
                        slice.transform.Rotate(new Vector3(0, angle, 0));

                        sameTypeCount -= 2;
                        continue;
                    }

                    if (sameTypeCount >= 1)
                    {
                        if (type != 3)
                            slice = Instantiate(circle_1of20, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);
                        else
                            slice = Instantiate(circle_1of20_escapeTrigger, new Vector3(0, startYPosition - platformDistance * i), Quaternion.identity, platformPieceContainer.transform);

                        SetSliceTagAndMaterial(slice, type);

                        if (i > 0 && type == 3 && sameTypeCount == 1 && movingDestroyPieceSize >= 3)
                            CreateMovingDestroyPlatform(-0.5f, -yRotationPerOneSlice * movingDestroyPieceSize, slice);

                        angle += yRotationPerOneSlice;
                        slice.transform.Rotate(new Vector3(0, angle, 0));

                        sameTypeCount -= 1;
                    }
                }

                // check if it is last piece
                if (type != rand && slices.Count == 0)
                {
                    sameTypeCount = 1;
                    type = rand;
                    goto whileStart;
                }

                // reset after creation of slice type
                type = rand;
                sameTypeCount = 1;
            }
        }
    }

    /// <summary>
    /// Creating moving destroy platform piece that rotates between given angles.
    /// </summary>
    /// <param name="startAngle">Angle where platform starts rotation.</param>
    /// <param name="endAngle">Angle where platform ends rotation.</param>
    /// <param name="parent">Object that this platform piece will be attached to.</param>
    /// <param name="risk">Risk is true by default, if risk is false, moving destroy platform will be created 100%, otherwise chance is 25%</param>
    void CreateMovingDestroyPlatform(float startAngle, float endAngle, GameObject parent, bool risk = true)
    {
        // 25% chance to create moving platform
        if (risk && Random.Range(0, 4) != 1)
            return;

        GameObject obj = Instantiate(smallMovingDestroyPlatform, parent.transform.position, Quaternion.identity, parent.transform);

        obj.GetComponent<MovingPlatformController>().endAngle = endAngle;
        obj.GetComponent<MovingPlatformController>().startAngle = startAngle;
    }

    /// <summary>
    /// Set material and tag to platform piece.
    /// </summary>
    /// <param name="slice">Piece that needs to be setup.</param>
    /// <param name="type">Type of that piece.</param>
    void SetSliceTagAndMaterial(GameObject slice, int type)
    {
        if (type == 1)
        {
            slice.tag = "safe";
            slice.GetComponent<MeshRenderer>().material = safePlatform;
        }
        else if (type == 2)
        {
            slice.tag = "destroy";
            slice.GetComponent<MeshRenderer>().material = destroyPlatform;
        }
        else if (type == 3)
            slice.tag = "escape";
    }

    /// <summary>
    /// Resets ball position and its variables.
    /// </summary>
    void ResetBall()
    {
        ball.transform.position = ballPosition;

        GameManager.instance.cameraController.target = ball;

        // stop all forces
        ball.GetComponent<Rigidbody>().isKinematic = true;
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.GetComponent<BallController>().DefineStart();
    }

    /// <summary>
    /// Destroying all platforms.
    /// </summary>
    void DestroyPlatforms()
    {
        GameObject[] pieceContainers = GameObject.FindGameObjectsWithTag("piece container");
        foreach (GameObject p in pieceContainers)
            Destroy(p);
    }

    /// <summary>
    /// Restarts level.
    /// </summary>
    public void RestartLevel()
    {
        ResetBall();
        DestroyPlatforms();
        CreatePlatforms();
    }

    /// <summary>
    /// Start new level.
    /// </summary>
    public void NewLevel()
    {
        RandomizeCollors();

        RestartLevel();
        ball.GetComponent<BallController>().breakWorth = GameManager.instance.level;
        ball.GetComponent<MeshRenderer>().material.color = ballColor;
    }

    /// <summary>
    /// Define materials colors.
    /// </summary>
    void SetCollors()
    {
        safePlatform.color = safeColor;
        destroyPlatform.color = destroyColor;
        cleanHitPlatformMaterial.color = ballColor;
        backgroundMaterial.color = backgroundColor;
        ballMaterial.color = ballColor;
        pillarMaterial.color = pilarColor;
    }

    /// <summary>
    /// Create random colors for materials.
    /// </summary>
    void RandomizeCollors()
    {
        safeColor = Random.ColorHSV(0, 1, 0, 1);
        destroyColor = Color.white - safeColor;

        backgroundColor = destroyColor - safeColor;

        ballColor = Random.ColorHSV(0, 1, 0, 1);
        pilarColor = Random.ColorHSV(0, 1, 0, 1);

        SetCollors();
    }
}
