-------------------------------------
--	b:	bool				bool
--	c:	signed char			int8_t
--	C:	unsigned char		uint8_t
--	s:	short				int16_t
--	S:	unsigned short		uint16_t
--	i:	int					int32_t
--	I:	unsigned int		uint32_t
--	d:	double				double
--	a:	string				string
-------------------------------------

--c2s
c2s.registerProto("myTest",         "i")


--s2c
s2c.registerProto("helloTest",      "bcCsSiIda")
s2c.registerProto("helloTest2",     "aa")
s2c.registerProto("helloTest3",     "a")
s2c.registerProto("helloTest4",     "ia")