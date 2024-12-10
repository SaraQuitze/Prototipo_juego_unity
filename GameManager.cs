using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool ejecutando;
    private bool cargandoNivel;
    private int indiceNivelInicio;
    //se crea una variable estática para asegurarnos que funciona solamente para esta clase y por la cual se podrá instanciar a otras clases, como PlayerController
    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player;
    public Text textoMonedas;
    public int monedas;
    public Text guardadoTexto;
    public bool avanzandoNivel;
    public int nivelActual;
    public List<Transform> posicionesAvance = new List<Transform>();
    public List<Transform> posicionesRetro = new List<Transform>();
    public List<Collider2D> areasCamara = new List<Collider2D>();
    public CinemachineConfiner cinemachineConfiner;
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;
    public GameObject panelTransicion;
    public GameObject panelInstrucciones;
    
    private void Awake()
    {
        //creamos una instancia con la cual se pueda asegurar que solo hay un Game Manager para todo el juego.
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        /*if(PlayerPrefs.GetInt("vidas") != 0)
        {
            cargarPartida();
        }*/
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Nivel1")//cambiar nombre escena niveles
        {
            nivelActual = PlayerPrefs.GetInt("indiceNivelInicio");
            indiceNivelInicio = PlayerPrefs.GetInt("indiceNivelInicio");
            posicionInicialJugador(indiceNivelInicio);
            cinemachineConfiner.m_BoundingShape2D = areasCamara[indiceNivelInicio];
        }
        else if(SceneManager.GetActiveScene().name == "LevelSelect")
        {
            posicionInicialJugador(0);
        }
    }

    public void activarPanelTransicion()
    {
        panelTransicion.GetComponent<Animator>().SetTrigger("ocultar");
    }

    private void posicionInicialJugador(int indiceNivelInicio)
    {
        player.transform.position = posicionesAvance[indiceNivelInicio].transform.position;
    }

    public void setIndiceInicio(int indiceNivelInicio)
    {
        this.indiceNivelInicio = indiceNivelInicio;
        PlayerPrefs.SetInt("indiceNivelInicio", indiceNivelInicio);
    }

    public void cambiarPosicionJugador()
    {
        if(avanzandoNivel)
        {
            if(nivelActual + 1 < posicionesAvance.Count)
            {//si hablamos de cambiar posición, entonces se entiende que los niveles se encuentran en la misma escena, si hablamos de cambiar escenas, entonces cada nivel se encuentra en diferentes escenas. En este caso, al manejar "position" estamos hablando de varios niveles en la misma escena. 
                player.transform.position = posicionesAvance[nivelActual + 1].transform.position;
                cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual + 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("Caminar", false);
                player.terminandoMapa = false;
            }
        }
        else
        {
            if(posicionesRetro.Count > nivelActual - 1)
            {
                player.transform.position = posicionesRetro[nivelActual - 1].transform.position;
                cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual - 1];
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("Caminar", false);
                player.terminandoMapa = false;
            }
        }
    }

    public void GuardarPartida()
    {
        float x, y; //posición del jugador al momento de guardar la partida
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;
        int nombreEscena = nivelActual;

        PlayerPrefs.SetInt("monedas", monedas);
        PlayerPrefs.SetFloat("x", x);
        PlayerPrefs.SetFloat("y", y);
        PlayerPrefs.SetInt("vidas", vidas);
        PlayerPrefs.SetInt("nivel", nombreEscena);
        PlayerPrefs.SetInt("indiceNivelInicio", indiceNivelInicio);

        if(!ejecutando)
        {
            StartCoroutine(MostratTextoGuardad());
        }
    }

    private IEnumerator MostratTextoGuardad()
    {
        ejecutando = true;
        guardadoTexto.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        guardadoTexto.gameObject.SetActive(false);
        ejecutando = false;
    }

    public void CargarNiveles(string nombreNivel)
    {
        SceneManager.LoadScene(nombreNivel);
    }

    public void cargarPartida()
    {
        monedas = PlayerPrefs.GetInt("monedas");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.vidas = PlayerPrefs.GetInt("vidas");
        textoMonedas.text = monedas.ToString();
        nivelActual = PlayerPrefs.GetInt("nivel");
        cinemachineConfiner.m_BoundingShape2D = areasCamara[nivelActual];
        indiceNivelInicio = PlayerPrefs.GetInt("indiceNivelInicio");   
        /*if(PlayerPrefs.GetString("nombreEscena") == string.Empty)
        {
            SceneManager.LoadScene("LevelSelect");
        }
        else
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("nombreEscena"));
        }*/     

        int vidasADescontar = 3 - player.maxvidas;
        int vidasARecuperar = 1 + player.maxvidas;

        player.MostrarVidasUI();
        player.ActualizarVidasUI(vidasADescontar);
        player.ActualizarVidasUIup(vidasARecuperar);
    }
    
    public void ActualizarContadorMonedas()
    {
        monedas ++;
        textoMonedas.text = monedas.ToString();
    }

    public void PausarJuego()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }

    public void instruccionesJuego()
    {
        Time.timeScale = 0;
        panelInstrucciones.SetActive(true);
    }

    public void DespausarJuego()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    public void salirInstruccion()
    {
        Time.timeScale = 1;
        panelInstrucciones.SetActive(false);
    }

    //MainMenu
    public void VolverMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void LvlSelector()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }

    public void CargarEscena(string escenaACargar)
    {
        StartCoroutine(CargarEscenaCorrutina(escenaACargar));
    }
    
    public IEnumerator CargarEscenaCorrutina(string escenaACargar)
    {
        cargandoNivel = true;
        SceneManager.LoadScene(escenaACargar);
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(escenaACargar);
        //esperar hasta que la escena cargue asincrónicamente
        while(!asyncLoad.isDone)
        {
            yield return null;
        }
        //posicionInicialJugador(indiceNivelInicio);
        cargandoNivel = false;
    }
    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }

    public void ContinuarJuego()
    {
        if(PlayerPrefs.GetFloat("x") != 0.0f)
        {
            player.enabled = true;
            cargarPartida();
            panelGameOver.SetActive(false);
        }
    }
    public void SalirdelJuego()
    {
        Application.Quit();
    }

    public void CargarEscenaSelector()
    {
        StartCoroutine(CargarEscena());
    }

    private IEnumerator CargarEscena()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelSelect");
        //esperar hasta que la escena cargue asincrónicamente
        while(!asyncLoad.isDone)
        {
            yield return null;//yield return new WaitForSeconds(200); se lo puede dejar en null para que la escena de carga aparezca la estricta cantidad de tiempo que se demora en cargar el juego o puede ponerse un new WaitForSeconds() con un tiempo adicional para que esta se pueda mostrar.
        }
    }
}
