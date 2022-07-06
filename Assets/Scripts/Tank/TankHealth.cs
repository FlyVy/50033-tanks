using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public GameObject Shield; // Shield object to be instantiated when invulnerable
    public bool Invulnerable; // boolean variable to check whether a tank will take damage
    public GameObject current_Shield; // Holds the instance of a shield
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public bool TakeDamage(float amount) // changed to bool function to return whether the damage kills or not
    {
        if(!Invulnerable){ // only take damage when not invulnerable
            // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
            m_CurrentHealth -= amount;

            SetHealthUI();
            if (m_CurrentHealth <= 0f && !m_Dead) {
                OnDeath();
                return true;
            }
            return false;
        }
        return false;
    }

    public void SetInvulnerable(int duration)
    {
        Invulnerable = true;
        current_Shield = Instantiate(Shield, this.transform.position, Quaternion.identity);
        current_Shield.GetComponent<AudioSource>().Play();
        current_Shield.transform.parent = this.gameObject.transform;
        current_Shield.transform.localScale = new Vector3(8.0f, 8.0f, 8.0f);
        StartCoroutine(removeInvulnerable(duration));
    }

    IEnumerator removeInvulnerable(int duration)
    {
        yield return new WaitForSeconds(duration);
        Invulnerable = false;
        Destroy(current_Shield);
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        gameObject.SetActive(false);
    }
}