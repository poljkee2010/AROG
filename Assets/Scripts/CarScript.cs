using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarScript : MonoBehaviour { //Названия основного класса всегда должно совпадать с названием файла скрипта

    WheelJoint2D[] wheelJoints; //Объявляем массив колёс
    JointMotor2D frontWheel;
    JointMotor2D backWheel;

    public float maxSpeed = -1000f; //макс.скорость колёс
    private float maxBackSpeed = 1500f;
    private float acceleration = 400f; //ускорение
    private float deacceleration = -100f; //скорость замедления (когда нажимаем на тормоз)
    public float brakeForce = 3000f; //тормозная сила
    private float gravity = 9.8f; //гравитация нашей земли (карты) как g в физике
    private float angleCar = 0; //угол наклона нашей машины (для вычисления скорости)
    public bool grounded = false;
	public LayerMask MAP; //слой карты
    public Transform Bwheel;
    private int coinsInt = 0; //подсчёт монет
    public Text coinsText; //числовой счётчик монет
    public float fuelSize; //весь бензин
    public float fuelUsage; //потребление бензина
    private float currentFuel; //бензин в настоящий момент (отображается в индикаторе)
    public GameObject fuelProgressBar; //индикатор топлива
    public GameObject centerMass;
    public float fuelAdd = 3; //количество добавляемого топлива при сборе топлива
    private AudioSource carSound;
	public AudioSource coinSound;

	public ClickScript[] ControlCar;

	// Use this for initialization
	void Start () { // метод Start() выполняется единожды, сразу после окончания загрузки сцены

        GetComponent<Rigidbody2D> ().centerOfMass = centerMass.transform.localPosition;
		wheelJoints = gameObject.GetComponents<WheelJoint2D>();
		backWheel = wheelJoints[1].motor;
		frontWheel = wheelJoints[0].motor;
		currentFuel = fuelSize;
		carSound = GetComponent<AudioSource>();
	}
	void Update() // Функция Update() вызывается единожды в каждом кадре
    {
        coinsText.text = coinsInt.ToString(); //перевод целого числа в строку (для счётчика монет)
        grounded = Physics2D.OverlapCircle(Bwheel.transform.position, 0.30f, MAP); //проверка на земле ли машина
    }
	void FixedUpdate() {

		if (currentFuel <= 0)   {  //если топливо закончилось
            print ("Закончилось топливо");
			return;
		}
		frontWheel.motorSpeed = backWheel.motorSpeed;

        angleCar = transform.localEulerAngles.z; //вдоль оси Z

        if (angleCar >= 180)
		{
			angleCar = angleCar - 360;
		}

        //работа с газом
        if (grounded == true) //если газ нажат,то появляется зависимость от угла машины и.т.д. т.е. ускорение в итоге будет гладкое
        {
            if (ControlCar[0].clickedIs == true)
            {
                backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (acceleration - gravity * Mathf.PI * (angleCar / 2)) * Time.fixedDeltaTime, maxSpeed, maxBackSpeed);
                currentFuel -= fuelUsage * Time.deltaTime; //минисуем бензин
            }
            else if ((backWheel.motorSpeed < 0) || (ControlCar[0].clickedIs == false && backWheel.motorSpeed == 0 && angleCar < 0))
            {
                backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (deacceleration - gravity * Mathf.PI * (angleCar / 2)) * Time.fixedDeltaTime, maxSpeed, 0);
                currentFuel -= (fuelUsage / 2.5f) * Time.deltaTime; //минисуем бензин
            }
            if ((ControlCar[0].clickedIs == false && backWheel.motorSpeed > 0) || (ControlCar[0].clickedIs == false && backWheel.motorSpeed == 0 && angleCar > 0))
                backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (-deacceleration - gravity * Mathf.PI * (angleCar / 2)) * Time.fixedDeltaTime, 0, maxBackSpeed);
        }
        else
        {
            if (ControlCar[0].clickedIs == false && backWheel.motorSpeed < 0) backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - deacceleration * Time.fixedDeltaTime, maxSpeed, 0);
            else if (ControlCar[0].clickedIs == false && backWheel.motorSpeed > 0) backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + deacceleration * Time.fixedDeltaTime, 0, maxBackSpeed);
            if (ControlCar[0].clickedIs == true)
            {
                backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (acceleration - gravity * (angleCar / 2)) * Time.fixedDeltaTime, maxSpeed, maxBackSpeed);
                currentFuel -= (fuelUsage /1.25f) * Time.deltaTime; //минисуем бензин
            }
        }

        //Работа с тормозом(тормозная система)
        if (ControlCar[1].clickedIs == true && backWheel.motorSpeed > 0)
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - brakeForce * Time.fixedDeltaTime, 0, maxBackSpeed);
        else if (ControlCar[1].clickedIs == true && backWheel.motorSpeed < 0)
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + brakeForce * Time.fixedDeltaTime, maxSpeed, 0);

        //Приравниваем скорость WheelJoints к JointMotor2D.
        wheelJoints[1].motor = backWheel;
		wheelJoints[0].motor = frontWheel;

		carSound.pitch = Mathf.Clamp (-backWheel.motorSpeed / 1000, 0.3f, 3);
        fuelProgressBar.transform.localScale = new Vector2(currentFuel / fuelSize, 1); //изменение индикатора бензина (делим текущий бензин на полный бак)
    }
    void OnTriggerEnter2D(Collider2D trigger) //функция ,что будет происходить при сборе монеток машиной и финише
    {
		if (trigger.gameObject.tag == "coins") {
            coinsInt++; //при каждом сборе монеты в игре будет прибавляться на единицу к счётчику
            coinSound.Play ();
            Destroy(trigger.gameObject);            //монетки будут разрушаться (их будет разрушаться gameObject,если быть точнее)
        }
        else if (trigger.gameObject.tag == "Finish") {
            SceneManager.LoadScene(0); //перезагрузка уровня
        }
        else if (trigger.gameObject.tag == "Fuel") {
			currentFuel += fuelAdd;
			Destroy (trigger.gameObject);
		}
	}
}
