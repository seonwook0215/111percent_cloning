using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [System.Serializable]
    public class Skill
    {
        public string skillName;
        public float cooldown;
        public GameObject prefab;
        public float duration;
        public float LastUsedTime => lastUsedTime;
        private float lastUsedTime;

        public bool CanUse()
        {
            return Time.time - lastUsedTime >= cooldown;
        }

        public void Use(GameObject caster)
        {
            if (!CanUse())
                return;

            lastUsedTime = Time.time;
            PlayerMove player = caster.GetComponent<PlayerMove>();
            switch (skillName)
            {
                case "Q":
                    Debug.Log("QSKillActivated");
                    if (player != null)
                    {
                        player.ActivateQSkill();
                    }
                    break;

                case "W":
                    GameObject dummy = GameObject.Instantiate(prefab, caster.transform.position, caster.transform.rotation);
                    dummy.GetComponent<DummyManager>().Init(caster);
                    GameObject.Destroy(dummy, duration);
                    break;

                case "E":
                    if (player != null)
                    {
                        player.ActivateESkill(1.5f, duration);
                    }
                    break;

                case "R":
                    if (player != null)
                    {
                        player.GenerateEarthquake();
                    }
                    break;
            }
        }
    }
    public Skill[] skills;

    public void UseSkill(int index, GameObject caster)
    {
        if (index < 0 || index >= skills.Length)
            return;
        skills[index].Use(caster);
        //Debug.Log("skill suces");
    }


}
