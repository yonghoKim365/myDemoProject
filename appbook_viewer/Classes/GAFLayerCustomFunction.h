#define CUSTOM_CREATE_FUNC(__TYPE__) \
	static __TYPE__* create(PhysicsWorld* world) \
{ \
	__TYPE__ *pRet = new __TYPE__(); \
if (pRet && pRet->init(world)) \
	{ \
	pRet->autorelease(); \
	return pRet; \
	} \
	 else \
	 { \
	 delete pRet; \
	 pRet = NULL; \
	 return NULL; \
	 } \
}
