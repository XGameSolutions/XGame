-- require "lib/driver"
-- c2s = xd.sender()
-- s2c = xd.reader()
-- require "proto/scProto"
require "utils/Namespace"
require "utils/Util"
require "utils/TableUtil"
require "utils/UEVectorUtil"

-- require "net/c2sBT"
-- require "net/s2cBT"
-- require "net/s2cTest"

require "CSharpCallLua"
require "launch/Launcher"

require "global/Config"

require "const/RoleConst"

require "basic/ui/UIConst"
require "basic/ui/UIUtil"
require "basic/ui/UIView"

require "core/GameMain"
require "core/assets/Assets"
require "core/assets/AssetHelper"
require "core/assets/Manifest"
require "core/assets/ABLoader"
require "core/assets/WebLoader"
require "core/scene/Scene"
require "core/map/Map"
require "core/camera/CameraMgr"

require "core/role/Role"
require "core/role/RoleGen"
require "core/role/RoleMove"
require "core/role/LocalPlayer"
require "core/role/LocalPlayerMove"
require "core/role/internal/RoleHelper"

require "core/input/Input"

require "modules/login/LoginView"

require "modules/loading/LoadingView"

require "modules/main/MainView"
