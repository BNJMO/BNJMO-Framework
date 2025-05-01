namespace BNJMO
{
    public abstract class AbstractManager : BBehaviour
    {
        /// <summary>
        /// Mark this object as should not be destroyed when a new scene is loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
