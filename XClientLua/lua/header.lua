require "lib/driver"

c2s = xd.sender()
s2c = xd.reader()

require "proto/scProto"
require "utils/namespace"
require "utils/tableTool"

require "net/c2sBT"
require "net/s2cBT"
require "net/s2cTest"