using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTool : MonoBehaviour
{
    [Header("References")]
    [field: SerializeField] PlayerEnergy playerEnergy;
    [field: SerializeField] PlayerSkillCrushAndRun playerSkill;
    [field: SerializeField] PlayerTriggerBody playerTriggerBody;
    [field: SerializeField] PresentGenerator presentGenerator;

    [Header("Prefab Particle")]
    [field: SerializeField] GameObject prefParticleLeaf;

    [field: Header("Particle Plane")]
    [field: SerializeField] GameObject particlePlane;

    [field: SerializeField] public bool IsToolCuttingGrass { private set; get; } = false;

    //Target
    SingleBush bushesTarget;

    void Awake()
    {
        playerSkill = GetComponentInParent<PlayerSkillCrushAndRun>();
        playerEnergy = GetComponentInParent<PlayerEnergy>();
        playerTriggerBody = transform.parent.GetComponentInChildren<PlayerTriggerBody>();
        presentGenerator = FindObjectOfType<PresentGenerator>();
    }

    // Start is called befores the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (bushesTarget == null)
        {
            return;
        }
        IsToolCuttingGrass = false;
        bool isDestroyBush = false;

        if (playerSkill.IsUseSkill)
        {
            isDestroyBush = bushesTarget.CutBushes(true);
        }
        else if (playerEnergy.ActionCutGrass(bushesTarget.EnergyConsumeValue))
        {
            IsToolCuttingGrass = true;
            isDestroyBush = bushesTarget.CutBushes();
        }

        if (isDestroyBush)
        {
            presentGenerator.GeneratePresent(bushesTarget.transform.position);
            playerSkill.AddSkillValue(bushesTarget.SkillGetValue);
            IsToolCuttingGrass = false;
            playerTriggerBody.IsInsideBush = false;
            //Particle
            GameObject obj = Instantiate(prefParticleLeaf);
            Vector3 newPos = bushesTarget.transform.position;
            newPos.y += 1.0f;
            //newPos += transform.forward * 2.0f;
            obj.transform.position = newPos;

            //ParticleSystem par = obj.GetComponent<ParticleSystem>();
            //par.collision.AddPlane(particlePlane.transform);
            //par.Play();
            Destroy(obj, 3.0f);

            //bushes.CheckDestroyBush();
            //IsToolInsideBush = false;
            //presentManager.GeneratePresent(other.transform.position);
            //Destroy(other.gameObject);
        }

        bushesTarget = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            bushesTarget = other.GetComponentInParent<SingleBush>();
        }
    }

}
