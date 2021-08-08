using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptAudioManager : MonoBehaviour
{
    static public scriptAudioManager Instance;

    public GameObject earth;
    public GameObject[] enemyPacks;
    public AudioClip[] musicLoops;

    public enum vibes
    {
        Chill,
        EnemyTerritory,
        Alerted,
        Dead,
        Victory
    }

    private vibes currentVibe;
    private vibes prevVibe;
    private bool vibeChange = false;

    //Coroutines
    private IEnumerator playLoopRoutine;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("An audio manager already exists in the scene!", Instance);
        }

        //Go find the earth
        var potentialEarths = GameObject.FindGameObjectsWithTag("Earth");
        if (potentialEarths.Length == 0)
            Debug.LogError("There is no earth object in the scene for the audio manager to reference.");
        else if (potentialEarths.Length > 1)
            Debug.LogError("There is more than 1 earth object in the scene.");
        else
            earth = potentialEarths[0];

        FindEnemyPacks();

        SetCurrentVibe(vibes.Chill);
    }

    // Update is called once per frame
    void Update()
    {
        //Chill vibe
        if (currentVibe == vibes.Chill)
            HandleChill();
        //Enemy territory
        else if (currentVibe == vibes.EnemyTerritory)
            HandleEnemyTerritory();
        //It's go time
        else if (currentVibe == vibes.Alerted)
            HandleAlerted();
        //You Died
        else if (currentVibe == vibes.Dead)
            HandleDead();
        //You Won!
        else if (currentVibe == vibes.Victory)
            HandleVictory();

        CheckUnits();
    }

    //sets the curent vibe and  caches the previous vibe
    public void SetCurrentVibe(vibes newVibe)
    {
        if (newVibe != currentVibe)
        {
            prevVibe = currentVibe; //cache the previous vibe
            currentVibe = newVibe;

            vibeChange = true;
        }
		else
		{
            vibeChange = false;
		}
    }

    public void FindEnemyPacks()
	{
        //Go find enemy packs
        var potentialPacks = GameObject.FindGameObjectsWithTag("EnemyPack");
        if (potentialPacks.Length == 0)
            Debug.LogWarning("There is no enemy packs in the scene.");
        else
            enemyPacks = potentialPacks;
    }

    void HandleChill()
	{
        if (vibeChange)
        {
            TransitionMusic();
        }

        if (playLoopRoutine == null)
		{
            playLoopRoutine = PlayLoopRoutine(earth.GetComponent<AudioSource>(), vibes.Chill);
            StartCoroutine(playLoopRoutine);
        }
	}

    void HandleEnemyTerritory()
	{
        if (vibeChange)
		{
            TransitionMusic();
		}
            
        if (playLoopRoutine == null)
		{
            playLoopRoutine = PlayLoopRoutine(earth.GetComponent<AudioSource>(), vibes.EnemyTerritory);
            StartCoroutine(playLoopRoutine);
        }
	}

    void HandleAlerted()
	{
        if (vibeChange)
        {
            TransitionMusic();
        }

        if (playLoopRoutine == null)
		{
            playLoopRoutine = PlayLoopRoutine(earth.GetComponent<AudioSource>(), vibes.Alerted);
            StartCoroutine(playLoopRoutine);
        }
	}

    void HandleDead()
	{
        if (vibeChange)
        {
            TransitionMusic();
        }

        if (playLoopRoutine == null && vibeChange) //only one-shot this.
		{
            playLoopRoutine = PlayLoopRoutine(earth.GetComponent<AudioSource>(), vibes.Dead);
            StartCoroutine(playLoopRoutine);
        }
	}

    void HandleVictory()
	{
        if (vibeChange)
        {
            TransitionMusic();
        }

        if (playLoopRoutine == null && vibeChange) //only one-shot this.
		{
            playLoopRoutine = PlayLoopRoutine(earth.GetComponent<AudioSource>(), vibes.Victory);
            StartCoroutine(playLoopRoutine);
        }
	}

    void CheckUnits()
    {
        vibes vibe = vibes.Chill;

        //How's the earth feeling?
        if (earth.GetComponent<scriptEarth>().currentState == scriptEarth.states.Dead)
		{
            vibe = vibes.Dead;
            SetCurrentVibe(vibe);
            return;
		}
        else if (earth.GetComponent<scriptEarth>().currentState == scriptEarth.states.Safe)
		{
            vibe = vibes.Victory;
            SetCurrentVibe(vibe);
            return;
        }

        //Check the enemy packs for their thoughts.
        foreach (GameObject pack in enemyPacks)
        {
            var packScript = pack.GetComponent<scriptEnemyPack>();

            if (packScript.aware == scriptEnemyPack.awareness.Alerted)
            {
                vibe = vibes.Alerted;
                break; //break out cause we're full force in this case
            }
            else if (packScript.aware == scriptEnemyPack.awareness.InTerritory)
            {
                vibe = vibes.EnemyTerritory;
            }
        }

        SetCurrentVibe(vibe);
    }

	void TransitionMusic()
	{
        StopCoroutine(playLoopRoutine);
        playLoopRoutine = null;
    }

    /// <summary>
	/// Adjusts the pitch of a given audio source per a ridgid body's velocity.
	/// </summary>
	/// <param name="audioSource">To change the pitch</param>
	/// <param name="rigbod">To check velocity</param>
	/// <param name="maxVelocity">The expected max speed of the rigidbody.</param>
	/// <param name="range">The range of pitch adjust. (Pitch can be set between -3 and 3 with 1 being no adjustment to the audio original pitch.</param>
	/// <param name="offset">Positions the range accordingly.</param>
	public void AdjustPitchBasedVelocity(AudioSource audioSource, Rigidbody rigbod, float maxVelocity, float range, float offset = 1)
    {
        if (rigbod != null)
        {
            var curVel = Mathf.Clamp(rigbod.velocity.sqrMagnitude, 0, maxVelocity);
            var ratio = ((curVel / maxVelocity) * range) + offset; //* 2 + 1 because pitch can go from 1 to 3.

            audioSource.pitch = Mathf.Lerp(audioSource.pitch, ratio, .1f);
        }
        else
            audioSource.pitch = Mathf.Lerp(audioSource.pitch, 1, .1f);
    }

    //Coroutines
    private IEnumerator PlayLoopRoutine(AudioSource anASrc, vibes vibe)
    {
        AudioClip audClip = musicLoops[(int)vibe];

        anASrc.clip = audClip;
        anASrc.Play();

        yield return new WaitForSeconds(audClip.length); //give the enemies time to run their dislodge scripts

        playLoopRoutine = null;
    }
}
