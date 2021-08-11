using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptAudioManager : MonoBehaviour
{
    static public scriptAudioManager Instance;
    public bool debugging = false;

    public GameObject earth;
    public GameObject[] enemyPacks;
    public AudioClip[] musicLoops;
    public bool deckToggle = true;//flips between two decks as we queue up tracks.
    public float transitionChill = .01f;
    public float transitionEnemyTerritory = .01f;
    public float transitionAlerted = .1f;
    public float transitionDead = .1f;
    public float transitionVictory = .1f;

    private AudioSource[] musicDecks;

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
    private Queue<Track> trackQueue = new Queue<Track>();

    //Coroutines
    private IEnumerator crossFadeRoutine;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("An audio manager already exists in the scene!", Instance);

        //Get our music decks
        var potentialASrcs = GetComponents<AudioSource>();
        if (potentialASrcs.Length == 0)
            Debug.LogError("No audio sources found on audio manager.", Instance);
        else if (potentialASrcs.Length > 2)
            Debug.LogError("Too many audio sources found on audio manager. Should only have 2.", Instance);
        else
            musicDecks = potentialASrcs;

        //Go find the earth
        var potentialEarths = GameObject.FindGameObjectsWithTag("Earth");
        if (potentialEarths.Length == 0)
            Debug.LogError("There is no earth object in the scene for the audio manager to reference.");
        else if (potentialEarths.Length > 1)
            Debug.LogError("There is more than 1 earth object in the scene.");
        else
            earth = potentialEarths[0];

        //Go find enemy packs
        FindEnemyPacks();

        currentVibe = vibes.Dead;//set the current vibe to something else just to trigger the first transition
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
        //You Died.
        else if (currentVibe == vibes.Dead)
            HandleDead();
        //You Won!
        else if (currentVibe == vibes.Victory)
            HandleVictory();

        HandleTrackQueue();

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

    void HandleTrackQueue()
	{
        if (trackQueue.Count > 0 && crossFadeRoutine == null)
		{
            var track = trackQueue.Peek();
            TransitionMusic(track.vibe, track.transitionSpeed, track.loop);
            trackQueue.Dequeue();
		}
	}

    void HandleChill()
	{
        if (vibeChange)
        {
            trackQueue.Enqueue(new Track { vibe = vibes.Chill, transitionSpeed = transitionChill, loop = true });
        }
	}

    void HandleEnemyTerritory()
	{
        if (vibeChange)
		{
            trackQueue.Enqueue(new Track { vibe = vibes.EnemyTerritory, transitionSpeed = transitionEnemyTerritory, loop = true });
		}
	}

    void HandleAlerted()
	{
        if (vibeChange)
        {
            trackQueue.Enqueue(new Track { vibe = vibes.Alerted, transitionSpeed = transitionAlerted, loop = true });
        }
	}

    void HandleDead()
	{
        if (vibeChange)
        {
            trackQueue.Enqueue(new Track { vibe = vibes.Dead, transitionSpeed = transitionDead, loop = false });
        }
	}

    void HandleVictory()
	{
        if (vibeChange)
        {
            trackQueue.Enqueue(new Track { vibe = vibes.Victory, transitionSpeed = transitionVictory, loop = false });
        }
	}

    void CheckUnits()
    {
        vibes vibe = vibes.Chill;

        //How's the earth feeling?
        if (scriptEarth.Instance.currentState == scriptEarth.states.Dead)
		{
            SetCurrentVibe(vibes.Dead);
            return;
		}
        else if (scriptEarth.Instance.currentState == scriptEarth.states.Safe)
		{
            SetCurrentVibe(vibes.Victory);
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

    /// <summary>
    /// Will cross-fade next music track based on the transitionSpeed.
    /// Some things need to cut in quickly while others need to ease in.
    /// </summary>
    /// <param name="vibe"></param>
    /// <param name="transitionSpeed">Lerp value (should be between 0 and 1).</param>
    /// <param name="loop">Launches the clip with loop mode enabled on the audio source.</param>
	void TransitionMusic(vibes vibe, float transitionSpeed = 1f, bool loop = false)
	{
        crossFadeRoutine = CrossFadeRoutine(vibe, transitionSpeed, loop);
        StartCoroutine(crossFadeRoutine);
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
    private IEnumerator CrossFadeRoutine(vibes vibe, float transitionSpeed, bool loop)
    {
        AudioClip audClip = musicLoops[(int)vibe];
        AudioSource deckToFadeOut = null;
        AudioSource deckToFadeIn = null;

        //Figure out which deck needs to come up and which down.
        if (!deckToggle)
		{
            deckToFadeIn = musicDecks[0];
            deckToFadeOut = musicDecks[1];
		}
		else
		{
            deckToFadeIn = musicDecks[1];
            deckToFadeOut = musicDecks[0];
        }
        deckToggle = !deckToggle;//Flip the deck toggle so we alternate which deck to crossfade to.

        //Set the new audio clip
        deckToFadeIn.clip = audClip;
        deckToFadeIn.loop = loop;
        deckToFadeIn.Play();

        var transitionValue = Mathf.Lerp(0, 1, transitionSpeed);

        //Cross fade
        while (deckToFadeIn.volume < 1 || deckToFadeOut.volume > 0)
		{
            deckToFadeIn.volume += transitionValue;
            deckToFadeOut.volume -= transitionValue;

            yield return null;
        }

        if (debugging) print("Stopping Deck.");
        deckToFadeOut.Stop();

        crossFadeRoutine = null;
    }
}

public class Track
{
    public scriptAudioManager.vibes vibe { get; set; }
    public float transitionSpeed { get; set; }
    public bool loop { get; set; }
}