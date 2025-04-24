using UnityEngine;

namespace BNJMO
{
    public class KillParticleSystem : BBehaviour
    {
        ParticleSystem myParitlce;
        AudioSource myAudio;

        protected override void OnEnable()
        {
            base.OnEnable();

            myParitlce = GetComponent<ParticleSystem>();
            myAudio = GetComponent<AudioSource>();
        }

        protected override void Update()
        {
            base.Update();

            if ((myParitlce != null) && (myAudio != null))
            // Particle, Audio
            {
                if ((myParitlce.IsAlive() == false) && (myAudio.isPlaying == false))
                {
                    Destroy(gameObject);
                }
            }
            else if ((myParitlce != null) && (myAudio == null))
            // No Particle, Audio
            {
                if (myParitlce.IsAlive() == false)
                {
                    Destroy(gameObject);
                }
            }
            else if ((myParitlce == null) && (myAudio != null))
            // Particle, No Audio
            {
                if (myAudio.isPlaying == false)
                {
                    Destroy(gameObject);
                }
            }
            else
            // No particle, No Audio
            {
                Destroy(gameObject);
            }
        }
    }
}