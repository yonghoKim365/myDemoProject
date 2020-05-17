using UnityEngine;
using System.Collections.Generic;

/*----------------------------------------------------------
 * NameSpace	: FSM
 * Author		: 윤주열, 김경호
 * Desc			: Unity3d 컴포넌트 기반 Finite State Machine.
 ----------------------------------------------------------*/
namespace FSM
{
	/*----------------------------------------------------------
	 * ClassName	: BaseState<PARENT>
	 * Author		: 윤주열, 김경호
	 ----------------------------------------------------------*/
	/// <summary>
	/// 상태 머신의 상태가 기본적으로 상속 받아야하는 클래스.
	/// virtual 선언된 함수들중 내용이 있는 함수는 상속받아 사용하는 클래스 단에서 호출 해줘야 함.
	/// </summary>
	/// <typeparam name="PARENT">상태를 가지는 객채</typeparam>
	public class BaseState<PARENT> : MonoBehaviour
	{
		///// <summary>
		///// 인스펙터 상에 비주얼 적으로 표시해주기 위한 플래그.
		///// </summary>
		//[SerializeField]
		//bool ActiveFlag = false;

		/// <summary>
		/// 상태를 가지는 클래스.
		/// </summary>
		public PARENT parent { get; private set; }

		/// <summary>
		/// Override하여 사용시 내부에서 [가장먼저] base로 접근하여 호출 되어야 함.
		/// </summary>
		public void Awake()
		{
			enabled = false;
			//ActiveFlag = false;
		}

		/// <summary>
		/// 상태가 추가되는 타임에 부모를 설정해줌.
		/// Override하여 사용시 내부에서 [가장먼저] base로 접근하여 호출 되어야 함.
		/// </summary>
		/// <param name="_parent"></param>
		virtual public void OnInitialize(PARENT _parent)
		{
			parent = _parent;
		}

		/// <summary>
		/// 상태 진입시점.
		/// Override하여 사용시 내부에서 [가장먼저] base로 접근하여 호출 되어야 함.
		/// </summary>
		virtual public void OnEnter(System.Action callback)
		{
			//enabled = ActiveFlag = true;
			enabled = true;

			if (null != callback)
				callback();
		}

		/// <summary>
		/// 상태 종료 시점.
		/// Override하여 사용시 내부에서 [마지막]으로 base로 접근하여 호출 되어야 함.
		/// </summary>
		virtual public void OnExit(System.Action callback)
		{
			//enabled = ActiveFlag = false;
			enabled = false;

			if (null != callback)
				callback();
		}

		/// <summary>
		/// 상태 파괴전 호출.
		/// Override하여 사용시 내부에서 [마지막]으로 base로 접근하여 호출 되어야 함.
		/// </summary>
		virtual public void OnRelease()
		{
			//enabled = ActiveFlag = false;
			enabled = false;
			Destroy( this );
		}

		virtual public void OnEvent(IBaseEvent evt) { }

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_PS3 || UNITY_IPHONE
		/// <summary>
		/// PARENT에서 OnGUI시 호출하여 사용.
		/// 에디터에서 확인/테스트 용도로 사용하기 위한 함수.
		/// 반듯이 #if UNITY_EDITOR 로 감싸서 코드분리를 확실히 해줘야 함.
		/// </summary>
		virtual public void Dev_OnGUI() { }
#endif

		//virtual public void OnOpenUI() { }
		//virtual public void OnCloseUI() { }

		virtual public void InputEvent(POINTER_INFO ptr) { }
	}


	/*----------------------------------------------------------
	 * ClassName	: FSM<EVENT, STATE, PARENT>
	 * Author		: 윤주열, 김경호
	 ----------------------------------------------------------*/
	/// <summary>
	/// 상태 머신 클래스.
	/// </summary>
	/// <typeparam name="EVENT">public enum 형식의 상태에 발동될 이벤트 목록</typeparam>
	/// <typeparam name="STATE">public enum 형식의 상태 목록</typeparam>
	/// <typeparam name="PARENT">상태를 가지는 객채</typeparam>
	public class FSM<EVENT, STATE, PARENT> : IEventHandler
	{
		PARENT mParent;

		public STATE Current_State { get; private set; }

		protected bool isEntering;

		Dictionary<STATE, Dictionary<EVENT, STATE>> TransitionMap;

		Dictionary<STATE, BaseState<PARENT>> StateMap;
		
		public BaseState<PARENT> Current()
		{
            if( !StateMap.ContainsKey(Current_State) )
            {
                Debug.LogError(Current_State);
                return null;
            }

			return StateMap[Current_State];
		}

		/// <summary>
		/// 생성자.
		/// </summary>
		/// <param name="parent">상태를 가지는 객채 자신(this)/</param>
		public FSM(PARENT parent)
		{
			mParent = parent;
			Initialize();
		}

		// 초기화.
		void Initialize()
		{
			TransitionMap = new Dictionary<STATE, Dictionary<EVENT, STATE>>();
			StateMap = new Dictionary<STATE, BaseState<PARENT>>();
		}

		// 릴리즈.
		public void Release()
		{
			StateMap[Current_State].OnExit( ReleaseCb );
		}

		void ReleaseCb()
		{
            foreach (BaseState<PARENT> ibs in StateMap.Values)
            {
                ibs.OnRelease();
            }
			StateMap.Clear();
			StateMap = null;
			TransitionMap.Clear();
			TransitionMap = null;
		}

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_PS3 || UNITY_IPHONE
		/// <summary>
		/// PARENT에서 OnGUI시 호출하여 사용.
		/// 에디터에서 확인/테스트 용도로 사용하기 위한 함수.
		/// 반듯이 #if UNITY_EDITOR 로 감싸서 코드분리를 확실히 해줘야 함.
		/// </summary>
		public void OnGUI_Dev()
		{
			StateMap[Current_State].Dev_OnGUI();
		}
#endif

		///// <summary>
		///// 상태에 UI오픈을 전달.
		///// </summary>
		//public void OnOpenUI()
		//{
		//    StateMap[Current_State].OnOpenUI();
		//}

		///// <summary>
		///// 상태에 UI 클로즈를 전달.
		///// </summary>
		//public void OnCloseUI()
		//{
		//    StateMap[Current_State].OnCloseUI();
		//}

        public bool GetState(STATE _STATE, out UnitStateBase state)
        {
            if (StateMap.ContainsKey(_STATE))
            {
                state = StateMap[_STATE] as UnitStateBase;
                return true;
            }

            state = null;
            return false;
        }

		public bool IsEnable { get { return EnableFlag; } }
		public bool EnableFlag = false;
		public void Enable(STATE state)
		{
            if (EnableFlag)
            {
                ChangeState( state );
            }
            else
            {
                Current_State = state;
			    Enable( true );   
            }
		}
		public void Enable(bool flag)
		{
			if (EnableFlag == flag)
				return;
			else EnableFlag = flag;

			if (EnableFlag)
			{
				if(StateMap.ContainsKey(Current_State))
					StateMap[Current_State].OnEnter( null );
			}
				
			else StateMap[Current_State].OnExit( null );
		}

		/// <summary>
		/// 상태 등록.
		/// </summary>
		/// <param name="_state">상태 값</param>
		/// <param name="_stateinterface">상태 클래스</param>
		public void AddState(STATE _state, BaseState<PARENT> _stateinterface)
		{
			StateMap.Add( _state, _stateinterface );
			_stateinterface.OnInitialize( mParent );
		}
		
		//이벤트 값을 얻어옴 ( 지금 상태에서 타겟 상태로 가기위한 이벤트가 있는지 확인 후 있으면 그 이벤트를 줌 )
		public EVENT GetEvent(STATE _BaseState, STATE _TargetState)
		{
			if(!TransitionMap.ContainsKey(_BaseState))
				return default(EVENT);
			
			foreach(EVENT e in TransitionMap[_BaseState].Keys)
			{
				if(TransitionMap[_BaseState][e].Equals(_TargetState))
				{
					return e;
				}
			}
			
			return default(EVENT);
		}

		/// <summary>
		/// 이벤트 등록.
		/// 적용할 상태에서 발생될 이벤트에 이동할 상태를 등록한다.
		/// </summary>
		/// <param name="_state">적용 할 상태</param>
		/// <param name="_event">발생될 이벤트</param>
		/// <param name="_targetstate">목표 상태</param>
		public void RegistEvent(STATE _state, EVENT _event, STATE _targetstate)
		{
			try
			{
				if (!TransitionMap.ContainsKey( _state ))
					TransitionMap.Add( _state, new Dictionary<EVENT, STATE>() );
				TransitionMap[_state].Add( _event, _targetstate );
			}
			catch (System.Exception e)
			{
				Debug.LogError( string.Format( "FiniteStateMap Add Error : {0}\nException : {1}", _event, e ) );
				Debug.Break();
			}
		}

		/// <summary>
		/// 이벤트를 발생시켜 현재 상태에 등록된 이벤트에 맞는 상태를 변경한다.
		/// </summary>
		/// <param name="_event">발생되는 이벤트</param>
		public bool ChangeState(EVENT _event)
		{
			if (isEntering)
			{
				Debug.LogWarning( string.Format( "Current : {0}, Target : {1} State Map Error.", Current_State, _event ) );
				Debug.LogWarning( "current state is already entering!" );
				return false;
			}

			if (TransitionMap[Current_State].ContainsKey( _event ))
			{
				//현재 상태와 동일 할경우 
				if (StateMap[Current_State].Equals( TransitionMap[Current_State][_event] ))
					return false;

				isEntering = true;
				StateMap[Current_State].OnExit( delegate()
				{
					Current_State = TransitionMap[Current_State][_event];
					StateMap[Current_State].OnEnter( delegate()
					{
						isEntering = false;
					} );
				} );
				return true;
			}
			else
			{
				Debug.LogWarning( string.Format( "Current : {0}, Target : {1} State Map Error.", Current_State, _event ) );
				return false;
			}
		}

        /// <summary>
        /// 이벤트 없이 상태전이를 바로 한다.
        /// </summary>
        /// <param name="_event">새로운 상태</param>
        public bool ChangeState(STATE _state)
        {
            if (isEntering)
            {
                Debug.LogWarning(string.Format("Current : {0}", Current_State));
                Debug.LogWarning("current state is already entering!");
                return false;
            }

            //현재 상태와 동일 할경우 
            if (Current_State.Equals(_state))
                return false;

            //< 해당 상태가 없을경우 패스
            if (!StateMap.ContainsKey(_state))
                return false;

            isEntering = true;
            StateMap[Current_State].OnExit(delegate()
            {
                Current_State = _state;
                StateMap[Current_State].OnEnter(delegate()
                {
                    isEntering = false;
                });
            });

            return true;
        }

		/// <summary>
		/// 이벤트가 등록되어있는지 확인한다.
		/// </summary>
		/// <param name="_event">확인할 이벤트.</param>
		/// <returns></returns>
		public bool CheckEvent(EVENT _event)
		{
			if (TransitionMap.ContainsKey( Current_State ) && TransitionMap[Current_State].ContainsKey( _event ))
			{
				if (StateMap[Current_State].Equals( TransitionMap[Current_State][_event] ))
				{
					
					return false;
				}
                else
				{
					
					return true;	
				}
			}
			
			return false;
		}

        public bool HasState(STATE state)
        {
            return StateMap.ContainsKey(state);
        }

		// for IEventHandler
		public bool IsEnabled { get; set; }
		public void OnEvent(IBaseEvent evt)
		{
			if (StateMap.ContainsKey( Current_State ))
				StateMap[Current_State].OnEvent( evt );
		}

		/// <summary>
		/// 상태에 인풋 이벤트를 발생시켜준다.
		/// </summary>
		/// <param name="evt"></param>
		public void InputEvent(POINTER_INFO ptr)
		{
			StateMap[Current_State].InputEvent( ptr );
		}
	}
}
