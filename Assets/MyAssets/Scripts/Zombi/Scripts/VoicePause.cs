using UnityEngine;
using System.Collections;


namespace FpsZomby { 
 class VoicePause : MonoBehaviour
    {
        Zombi zombie;
        public AudioSource voiceSound; // ������ �� AudioSource
        public float pauseDuration = 3f; // ������������ ����� � ��������
        private bool isCoroutineRunning = false;

        private void Start()
        {
            zombie = gameObject.GetComponent<Zombi>();

            if (voiceSound == null)
            {
                Debug.LogError("AudioSource �� ��������!");
                return;
            }
        }

        private void Update()
        {
            // ��������� �������� ��� ��������������� ����� � �������
            if (zombie.statusZombi == Zombi.ZombiStatus.run && !isCoroutineRunning)
            {
                StartCoroutine(PlaySoundWithPause());
            }
            // ������������� ����, ���� ������ ����� Dead
            if (zombie.statusZombi == Zombi.ZombiStatus.dead && voiceSound.isPlaying)
            {
                voiceSound.Stop();
            }
        }

        private IEnumerator PlaySoundWithPause()
        {
            isCoroutineRunning = true;
            while (zombie.statusZombi != Zombi.ZombiStatus.dead) // ��������� ������ �����
            {
                // ������������� ����
                voiceSound.Play();

                // ����, ���� ���� ����������
                yield return new WaitForSeconds(voiceSound.clip.length);

                // ����� ����� ��������� ����������������
                yield return new WaitForSeconds(pauseDuration);
            }
            isCoroutineRunning = false;
        }
    }

    

}
