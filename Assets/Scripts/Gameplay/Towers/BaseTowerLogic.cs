using System;
using Gameplay.BoardPieces;
using Gameplay.Towers.Ammo;
using UnityEngine;
using Utilities;

namespace Gameplay.Towers
{
    public class BaseTowerLogic : BoardPiece
    {
        // TODO: Buff system rework

        [Header("Tower Starting Stats")]
        [SerializeField] private float towerRange;

        [Header("Range Circle Material")]
        [SerializeField] private Material towerRangeCircleMaterial;

        [Header("Tower Ammo Prefab")]
        [SerializeField] private GameObject ammoPrefab;

        [Header("Tower Sound Effects")]
        [SerializeField] private AudioClip towerFireSoundEffect;
        [SerializeField] private AudioClip towerHighlightSoundEffect;

        // Tower behavior variables
        private GameObject ammoSpawnPoint;

        // Creep kill stats
        private int creepKillCounter;
        private float nextFireTime;
        private LineRenderer towerRangeCircle;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            InitializeAmmoSpawnPoint();
            InitializeTowerRangeCircle();
        }

        // Update is called once per frame
        private void Update()
        {
            if (GameUtils.IsGameInProgress() && !GameUtils.IsGamePaused())
            {
                var towerPosition = new Vector3(transform.position.x, 0, transform.position.z);

                foreach (var creep in GameUtils.GetWaveManager().creepList)
                {
                    var creepPosition = new Vector3(creep.transform.position.x, 0, creep.transform.position.z);

                    if (Vector3.Distance(towerPosition, creepPosition) < towerRange)
                    {
                        RotateTowardsTarget(creep);
                        TowerShoot(creep);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Locates the AmmoSpawnPoint GameObject within the tower hierarchy and stores it
        /// </summary>
        /// <exception cref="ArgumentNullException"> If tower GameObject does not have an AmmoSpawnPoint child object </exception>
        private void InitializeAmmoSpawnPoint()
        {
            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var childObj = gameObject.transform.GetChild(i).gameObject;

                if (childObj.name == "AmmoSpawnPoint")
                {
                    ammoSpawnPoint = childObj;
                    break;
                }
            }

            if (ammoSpawnPoint == null) throw new ArgumentNullException("AmmoSpawnPoint is null");
        }

        /// <summary>
        ///     Initializes the line renderer used to draw a circle around the tower representing the tower's radius
        /// </summary>
        private void InitializeTowerRangeCircle()
        {
            var circlePositionCount = 60;

            towerRangeCircle = gameObject.GetComponent<LineRenderer>();
            towerRangeCircle.positionCount = circlePositionCount + 1;
            towerRangeCircle.material = towerRangeCircleMaterial;
            towerRangeCircle.widthMultiplier = 0.1f;
            towerRangeCircle.useWorldSpace = false;

            // Populate list of positions in circle around tower
            var dtheta = (float) (2 * Math.PI / circlePositionCount);
            float theta = 0;
            for (var i = 0; i < circlePositionCount; i++)
            {
                var lineSegmentPosition = new Vector3(
                    (float) (0 + towerRange * Math.Cos(theta)),
                    transform.position.y,
                    (float) (0 + towerRange * Math.Sin(theta)));

                towerRangeCircle.SetPosition(i, lineSegmentPosition);

                if (i == 0) towerRangeCircle.SetPosition(circlePositionCount, lineSegmentPosition);

                theta += dtheta;
            }
        }

        /// <summary>
        ///     Rotates the tower to face the given creep
        /// </summary>
        /// <param name="targetCreep"> Creep that we want the tower to rotate towards </param>
        private void RotateTowardsTarget(GameObject targetCreep)
        {
            var heading = targetCreep.transform.position - transform.position;
            var heading2d = new Vector2(heading.x, heading.z).normalized;
            var angle = Mathf.Atan2(heading2d.y, heading2d.x) * -Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        }

        /// <summary>
        ///     Fires the tower's ammo prefab at the given creep
        /// </summary>
        /// <param name="targetCreep"> Creep that we want to shoot the ammo towards </param>
        private void TowerShoot(GameObject targetCreep)
        {
            if (Time.time > nextFireTime)
            {
                nextFireTime = Time.time + attackSpeed;
                Attack(targetCreep);
            }
        }

        /// <summary>
        ///     Define What the tower does once it's time to attack
        /// </summary>
        /// <param name="targetCreep"></param>
        protected virtual void Attack(GameObject targetCreep)
        {
            var newAmmo = Instantiate(ammoPrefab, ammoSpawnPoint.transform.position, ammoPrefab.transform.rotation);
            newAmmo.GetComponent<BaseAmmoLogic>().InitializeAmmo(this, targetCreep);
            if (GameSettingsUtils.IsSoundEnabled()) GetComponent<AudioSource>().PlayOneShot(towerFireSoundEffect);
        }

        /// <summary>
        ///     Toggles the tower's range circle
        /// </summary>
        /// <param name="toggle"> Enable or disable drawing the tower range circle </param>
        public void HighlightTowerRange(bool toggle)
        {
            if (towerRangeCircle == null) return;

            if (towerRangeCircle.enabled != toggle)
            {
                towerRangeCircle.enabled = toggle;

                if (toggle)
                    if (GameSettingsUtils.IsSoundEnabled())
                        GetComponent<AudioSource>().PlayOneShot(towerHighlightSoundEffect);
            }
        }

        /// <summary>
        ///     Logic triggered when this tower kills something
        /// </summary>
        /// <param name="victim"> BoardPiece object for the thing that was killed </param>
        public override void Killed(BoardPiece victim)
        {
            base.Killed(victim);
            creepKillCounter += 1;
        }
    }
}