using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private AudioController audioController;

    [SerializeField]
    public Path path;

    [Header("GameObjects")]
    public GameObject pathPrefab;
    public Transform pathHolder;
    public GameObject selectableIndicator;

    [Header("AudioClips")]
    public AudioClip buildPathClip;

    private bool isSelectable = false;

    public void Start() {
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
    }

    public void Initialize(Path newPath, float radius)
    {
        path = newPath;

        transform.GetComponent<BoxCollider>().size = new Vector3(radius/2, 1.5f, radius/2);
    }

    public void SetSelectable(bool selectable) {
        isSelectable = selectable;
        transform.GetComponent<BoxCollider>().enabled = selectable;
        selectableIndicator.SetActive(selectable);
    }

    public void BuildPath(Player player)
    {
        path.occupiedBy = player.id;

        Vector3 between = path.between.Item2.position - path.between.Item1.position;
        var angle = (float)(Mathf.Atan2(between.x, between.z) * Mathf.Rad2Deg + 90f);

        // Instantiate path object
        GameObject pathObject = GameObject.Instantiate(pathPrefab, transform.position, Quaternion.Euler(0, angle, 0));
        pathObject.transform.SetParent(pathHolder.transform);
        pathObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", player.GetColor());

        PlaySound();
    }

    public void PlaySound() {
        if(buildPathClip == null) {
            return;
        }

        audioController.PlayClip(buildPathClip);
    }
}
