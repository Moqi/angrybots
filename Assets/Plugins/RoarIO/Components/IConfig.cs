using System.Collections;

namespace Roar.Components
{
  /**
   * \brief IConfig is an interface for setting roar client configuration.
   *
   * @todo: The config interface could benefit from a little more functionality - a config getter, config dump etc.
   **/
  public interface IConfig
  {
    /**
     * Set a roar config key/value.
     * @param k the roar config key.
     * @param v the roar config value.
     **/
    void setVal( string k, string v);
  }
}

