using UnityEngine;
using System.Collections;


namespace FpsZomby { 
 class VoicePause : MonoBehaviour
    {
        Zombi zombie;
        public AudioSource voiceSound; // Ссылка на AudioSource
        public float pauseDuration = 3f; // Длительность паузы в секундах
        private bool isCoroutineRunning = false;

        private void Start()
        {
            zombie = gameObject.GetComponent<Zombi>();

            if (voiceSound == null)
            {
                Debug.LogError("AudioSource не назначен!");
                return;
            }
        }

        private void Update()
        {
            // Запускаем корутину для воспроизведения звука с паузами
            if (zombie.statusZombi == Zombi.ZombiStatus.run && !isCoroutineRunning)
            {
                StartCoroutine(PlaySoundWithPause());
            }
            // Останавливаем звук, если статус зомби Dead
            if (zombie.statusZombi == Zombi.ZombiStatus.dead && voiceSound.isPlaying)
            {
                voiceSound.Stop();
            }
        }

        private IEnumerator PlaySoundWithPause()
        {
            isCoroutineRunning = true;
            while (zombie.statusZombi != Zombi.ZombiStatus.dead) // Проверяем статус зомби
            {
                // Воспроизводим звук
                voiceSound.Play();

                // Ждем, пока звук закончится
                yield return new WaitForSeconds(voiceSound.clip.length);

                // Пауза перед следующим воспроизведением
                yield return new WaitForSeconds(pauseDuration);
            }
            isCoroutineRunning = false;
        }
    }

    

}
