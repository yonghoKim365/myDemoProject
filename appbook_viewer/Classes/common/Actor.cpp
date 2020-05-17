#include "../common/Actor.h"

USING_NS_CC;

//using namespace ho1b04;
using namespace experimental;


//namespace ho1b04{

	Actor::Actor() : m_pActor(NULL), m_pInitPos(NULL)
	{
		m_pActor = NULL;
		m_pInitPos = new Vec3(0, 0, 0);

		m_zeroVec3 = Vec3(0, 0, 0);
		m_crntDisplayPos = new Vec3(0, 0, 0);

		m_isOnGrnd	= 0;
		m_fVrtSpeed = 0;
		m_fHozSpeed = 0;
	}

	Actor::~Actor()
	{
		if (m_pActor != NULL){
			delete	m_pActor;
		}

		if (m_pInitPos != NULL){
			delete	m_pInitPos;
		}

		if (m_crntDisplayPos != NULL){
			delete	m_crntDisplayPos;
		}
		m_pActor = NULL;
		m_pInitPos = NULL;
		m_crntDisplayPos = NULL;
	}

	void Actor::init(GAFObject * s_gaf)
	{
		CCLOG("Actor::init(0)  m_pInitPos x: %f", m_pInitPos->x );
		m_pActor = s_gaf;


		//	this로서 schedule을 호출하면 실행이 안 되고,
		//	m_pActor로서 schedule을 호출하면 onInitPos() 내에서 m_pActor 등의 값이 모두 NULL 상태이다.
		//m_pActor->schedule(schedule_selector(Actor::onInitPos, 1));


		ActionInterval* delay = DelayTime::create(0.01f);
		ActionInterval* seq = Sequence::create(delay, CallFunc::create(CC_CALLBACK_0(Actor::onInitPos, this)), NULL);
		m_pActor->runAction(seq);
	}

	//void Actor::onInitPos( float dt)
	void Actor::onInitPos()
	{
		//	addChild 한 직후에 위치값을 읽으면 무조건 0 값을 얻게 될 것이다.
		//	초기 위치는 m_pInitPos와 같다.
		//	하지만, 이후 위치 설정을 해 줄 때는 입력값에서 m_pInitPos 만큼을 빼야 한다.
		Vec3	t_frmVec = Vec3(0, 0, 0);
		//m_pInitPos = new Vec3(0, 0, 0);
		//m_pActor->getNodeToWorldTransform().transformPoint(t_frmVec, m_pInitPos);

		float *t_x	= new float();
		float *t_y = new float();
		m_pActor->getPosition(t_x, t_y);


		//	2016. 4.23 duckbest: 초기값을 world 좌표가 아닌 부모로부터의 상대좌표로 설정함.
		m_pInitPos = new Vec3(0, 0, 0);
		//m_pActor->getParentToNodeTransform().transformPoint(t_frmVec, m_pInitPos);	//	나를 기준으로 부모에게로의 좌표.
		m_pActor->getNodeToParentTransform().transformPoint(t_frmVec, m_pInitPos);	//	부모를 기준으로 나에게로의 좌표.
		
		CCLOG("Actor::onInitPos 0   m_pInitPos->x: %f", m_pInitPos->x);


		//m_pInitPos->x = t_testToVec->x;
		//m_pInitPos->y = t_testToVec->y;
		//m_pInitPos->z = t_testToVec->z;

		m_crntDisplayPos->x = m_pInitPos->x;
		m_crntDisplayPos->y = m_pInitPos->y;
		m_crntDisplayPos->z = m_pInitPos->z;



		CCLOG("Actor::onInitPos 1   m_pInitPos: %f  %f  %f   pos X: %f  Y: %f", m_pInitPos->x, m_pInitPos->y, m_pInitPos->z, *t_x, *t_y);

	}


	GAFObject*	Actor::getGAFObject()
	{
		return	m_pActor;
	}

	Vec3* Actor::getInitDisplayPosition()
	{
		return	m_pInitPos;
	}

	Vec3* Actor::getDisplayPosition()
	{
		
		//Vec3*	t_vec = new Vec3();
		//t_vec->x = m_crntDisplayPos->x - m_pInitPos->x;
		//t_vec->y = m_crntDisplayPos->y - m_pInitPos->y;
		//t_vec->z = m_crntDisplayPos->z - m_pInitPos->z;
		//CCLOG("Actor::getPosition(0)   m_crntDisplayPos: %f, %f", t_vec->x, t_vec->y);
		return	m_crntDisplayPos;
	}
	void Actor::setDisplayPosition(float x, float y)
	{
		m_crntDisplayPos->x = x;
		m_crntDisplayPos->y = y;
		updatePos();
	}
	void Actor::setDisplayPositionX(float x)
	{
		m_crntDisplayPos->x = x;
		updatePos();
	}
	void Actor::setDisplayPositionY(float y)
	{
		m_crntDisplayPos->y = y;
		updatePos();
	}

	void Actor::moveX(float x)
	{
		m_crntDisplayPos->x += x;
		//CCLOG("Actor::moveX(0)   m_crntDisplayPos: %f %f", m_crntDisplayPos->x, m_crntDisplayPos->y);
		updatePos();
	}

	void Actor::moveY(float y)
	{
		m_crntDisplayPos->y += y;
		updatePos();
	}

	void Actor::moveXY(float x, float y)
	{
		m_crntDisplayPos->x += x;
		m_crntDisplayPos->y += y;
		updatePos();
	}

	void Actor::updatePos()
	{
		//CCLOG("Actor::updatePos(0)   m_crntDisplayPos: %f %f   m_pInitPos: %f %f", m_crntDisplayPos->x, m_crntDisplayPos->y, m_pInitPos->x, m_pInitPos->y);
		m_pActor->setPosition(m_crntDisplayPos->x - m_pInitPos->x, m_crntDisplayPos->y - m_pInitPos->y);
		//m_pActor->setPosition(m_crntDisplayPos->x, m_crntDisplayPos->y);
	}

	void Actor::setLooped(bool looped, bool recursive )
	{
		m_pActor->setLooped(looped, recursive);

	}

	void Actor::playSequence(const std::string& name, bool looped, bool resume /*= true*/)
	{
		if (crntSeqName == name && isLoop == looped && isResume == resume){
			return;
		}

		crntSeqName = name;
		isLoop = looped;
		isResume = resume;
		m_pActor->playSequence(name, looped, resume);
	}

	std::string& Actor::getCurrentSequenceName()
	{
		return	crntSeqName;
	}

	//	check whether acter is on ground or not.
	bool Actor::getIsOnGround()
	{
		return	m_isOnGrnd;
	}
	void Actor::setIsOnGround( bool flag)
	{
		m_isOnGrnd = flag;
	}
	

	float Actor::getVerticalSpeed()
	{
		return	m_fVrtSpeed;
	}
	void Actor::setVerticalSpeed(float f)
	{
		m_fVrtSpeed = f;
	}
	void Actor::addVerticalSpeed(float f)
	{
		m_fVrtSpeed += f;
	}


	float Actor::getHorizontalSpeed()
	{
		return	m_fHozSpeed;
	}
	void Actor::setHorizontalSpeed(float f)
	{
		m_fHozSpeed = f;
	}
	void Actor::addHorizontalSpeed(float f)
	{
		m_fHozSpeed += f;
	}


	bool Actor::getVisible()
	{
		if (m_pActor != NULL){
			return	m_pActor->isVisible();
		}
		return	false;
	}
	void Actor::setVisible( bool flag )
	{
		m_pActor->setVisible( flag );
	}



	bool Actor::checkContainsPoint(Point s_wrldPnt){
		GAFObject	*t_plrChkBox = m_pActor->getObjectByName("hitBox_mc");
		Vec3	t_frmVec = Vec3();
		Vec3	t_toVec = Vec3();
		t_plrChkBox->getNodeToWorldTransform().transformPoint(t_frmVec, &t_toVec);
		cocos2d::Rect	t_plrChkBoxRct = t_plrChkBox->getBoundingBox();
		cocos2d::Rect	t_plrChkBoxWrldRct = Rect(t_toVec.x, t_toVec.y, t_plrChkBoxRct.size.width, t_plrChkBoxRct.size.height);
		//CCLOG("Actor::checkContainsPoint(0.1)  t_toVec: %f %f    origin: %f %f   size: %f %f   t_plrChkBoxWrldRct: %f %f %f %f", t_toVec.x, t_toVec.y, t_plrChkBoxWrldRct.origin.x, t_plrChkBoxWrldRct.origin.y, t_plrChkBoxWrldRct.size.width, t_plrChkBoxWrldRct.size.height, t_plrChkBoxWrldRct.getMinX(), t_plrChkBoxWrldRct.getMinY(), t_plrChkBoxWrldRct.getMaxX(), t_plrChkBoxWrldRct.getMaxY());
		//CCLOG("Actor::checkContainsPoint(0.2)   loc: %f %f   box x,y: %f %f    size w: %f", touch->getLocation().x, touch->getLocation().y, t_plrChkBox->getBoundingBox().origin.x, t_plrChkBox->getBoundingBox().origin.y, t_plrChkBox->getBoundingBox().size.width);

		if (t_plrChkBoxWrldRct.containsPoint(s_wrldPnt)){
			CCLOG("Actor::checkContainsPoint(0.3)   hit!!!");
			return	true;
		}

		
		return	false;
	}

	bool Actor::checkContainsActor(Actor * s_actor){
		GAFObject	*t_plrChkBox = m_pActor->getObjectByName("hitBox_mc");
		Vec3	t_frmVec = Vec3();
		Vec3	t_toVec = Vec3();
		t_plrChkBox->getNodeToWorldTransform().transformPoint(t_frmVec, &t_toVec);
		cocos2d::Rect	t_plrChkBoxRct = t_plrChkBox->getBoundingBox();
		cocos2d::Rect	t_plrChkBoxWrldRct = Rect(t_toVec.x, t_toVec.y, t_plrChkBoxRct.size.width, t_plrChkBoxRct.size.height);
		//CCLOG("Actor::checkContainsPoint(0.1)  t_toVec: %f %f    origin: %f %f   size: %f %f   t_plrChkBoxWrldRct: %f %f %f %f", t_toVec.x, t_toVec.y, t_plrChkBoxWrldRct.origin.x, t_plrChkBoxWrldRct.origin.y, t_plrChkBoxWrldRct.size.width, t_plrChkBoxWrldRct.size.height, t_plrChkBoxWrldRct.getMinX(), t_plrChkBoxWrldRct.getMinY(), t_plrChkBoxWrldRct.getMaxX(), t_plrChkBoxWrldRct.getMaxY());
		//CCLOG("Actor::checkContainsPoint(0.2)   loc: %f %f   box x,y: %f %f    size w: %f", touch->getLocation().x, touch->getLocation().y, t_plrChkBox->getBoundingBox().origin.x, t_plrChkBox->getBoundingBox().origin.y, t_plrChkBox->getBoundingBox().size.width);


		GAFObject	*t_tarChkBox = s_actor->getGAFObject()->getObjectByName("hitBox_mc");
		t_frmVec = Vec3();
		t_toVec = Vec3();
		t_tarChkBox->getNodeToWorldTransform().transformPoint(t_frmVec, &t_toVec);
		cocos2d::Rect	t_tarChkBoxRct = t_tarChkBox->getBoundingBox();
		cocos2d::Rect	t_tarChkBoxWrldRct = Rect(t_toVec.x, t_toVec.y, t_tarChkBoxRct.size.width, t_tarChkBoxRct.size.height);
		CCLOG("Actor::checkContainsActor 0   this minX: %f   maxX: %f  minY: %f   maxY: %f", t_plrChkBoxWrldRct.getMinX(), t_plrChkBoxWrldRct.getMaxX(), t_plrChkBoxWrldRct.getMinY(), t_plrChkBoxWrldRct.getMaxY());
		CCLOG("Actor::checkContainsActor 1   tar minX: %f   maxX: %f  minY: %f   maxY: %f", t_tarChkBoxWrldRct.getMinX(), t_tarChkBoxWrldRct.getMaxX(), t_tarChkBoxWrldRct.getMinY(), t_tarChkBoxWrldRct.getMaxY());

		if (((t_plrChkBoxWrldRct.getMinX() < t_tarChkBoxWrldRct.getMinX() && t_plrChkBoxWrldRct.getMaxX() > t_tarChkBoxWrldRct.getMinX()) ||
			(t_plrChkBoxWrldRct.getMinX() < t_tarChkBoxWrldRct.getMaxX() && t_plrChkBoxWrldRct.getMaxX() > t_tarChkBoxWrldRct.getMaxX()) ||
			(t_plrChkBoxWrldRct.getMinX() > t_tarChkBoxWrldRct.getMinX() && t_plrChkBoxWrldRct.getMaxX() < t_tarChkBoxWrldRct.getMaxX())) &&

			((t_plrChkBoxWrldRct.getMinY() < t_tarChkBoxWrldRct.getMinY() && t_plrChkBoxWrldRct.getMaxY() > t_tarChkBoxWrldRct.getMinY()) ||
			(t_plrChkBoxWrldRct.getMinY() < t_tarChkBoxWrldRct.getMaxY() && t_plrChkBoxWrldRct.getMaxY() > t_tarChkBoxWrldRct.getMaxY()) ||
			(t_plrChkBoxWrldRct.getMinY() > t_tarChkBoxWrldRct.getMinY() && t_plrChkBoxWrldRct.getMaxY() < t_tarChkBoxWrldRct.getMaxY()))
			){

			return	true;
		}
		
		return	false;
	}

	void Actor::setScaleTo(float scale)
	{
		m_pActor->setScale(scale, scale);
	}

	float Actor::getScaleTo()
	{
		return	m_pActor->getScale();
	}


	void Actor::setAnimationRunning(bool value, bool recursive)
	{
		m_pActor->setAnimationRunning(value, recursive);
	}

	bool Actor::getIsAnimationRunning()
	{
		return	m_pActor->getIsAnimationRunning();
	}


	cocos2d::Vec3 Actor::getWorldTransformDisplayPoint(){
		Vec3	t_frmVec = Vec3();
		Vec3	t_toVec = Vec3();
		m_pActor->getNodeToWorldTransform().transformPoint(t_frmVec, &t_toVec);
		return	t_toVec;
	}

	cocos2d::Vec3 Actor::convertToLocalPoint(Vec3 s_wrldPnt){
		Vec3	t_tarWrldVec = getWorldTransformDisplayPoint();
		Vec3	t_elapseVec = Vec3(s_wrldPnt.x - t_tarWrldVec.x, s_wrldPnt.y - t_tarWrldVec.y, s_wrldPnt.z - t_tarWrldVec.z);
		Vec3	t_locVec = Vec3(t_elapseVec.x + getDisplayPosition()->x, t_elapseVec.y + getDisplayPosition()->y, t_elapseVec.z + getDisplayPosition()->z);
		CCLOG("Actor::convertToLocalPoint 0  s_wrldPnt.x: %f  t_tarWrldVec.x: %f  t_elapseVec.x: %f  t_locVec.x: %f  m_pInitPos->x: %f  getDisplayPosition()->x: %f", s_wrldPnt.x, t_tarWrldVec.x, t_elapseVec.x, t_locVec.x, m_pInitPos->x, getDisplayPosition()->x);

		return	t_locVec;

	}


	//cocos2d::Rect	getWorldTransformRect(GAFObject	* s_gafBox){
	//	Vec3	t_frmVec = Vec3();
	//	Vec3	t_toVec = Vec3();
	//	s_gafBox->getNodeToWorldTransform().transformPoint(t_frmVec, &t_toVec);
	//	cocos2d::Rect	t_rngBoxRct = s_gafBox->getBoundingBox();
	//	cocos2d::Rect	t_rngBoxWrldRct = Rect(t_toVec.x, t_toVec.y, t_rngBoxRct.size.width, t_rngBoxRct.size.height);

	//	return	t_rngBoxWrldRct;
	//}

//}