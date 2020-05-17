using System;
using System.IO;
using System.Reflection;
using Google.Protobuf;
using UnityEngine;

namespace Core.Net
{
    /// <summary>
    /// 消息解码器
    /// @author fanflash.org
    /// </summary>
    public class MsgDecoder
    {
        public Type BuilderType { get; private set; }
        private MethodInfo mParseFromMi;

        private MsgDecoder()
        {
            
        }

        /// <summary>
        /// decode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public IMessage Decode(Stream input, out string error)
        {
            try
            {
                IMessage msg = mParseFromMi.Invoke(null, new object[] { input }) as IMessage;
                error = null;
                return msg;

                /*
                IBuilderLite builder = mCreateBuilderMi.Invoke(null, null) as IBuilderLite;
                if (builder == null)
                {
                    error = "create builder is null";
                    return null;
                }
                 * 
                CodedInputStream codedInput = CodedInputStream.CreateInstance(input);
                builder.WeakMergeFrom(codedInput);
                codedInput.CheckLastTagWas(0);
                IMessage msg = builder.WeakBuild();
                error = null;
                return msg;
                */
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return null;
            }
        }

        /// <summary>
        /// decode
        /// if error, use Debug.LogError
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IMessage Decode(Stream input)
        {
            string error;
            IMessage msg = Decode(input, out error);
            if (msg == null)
            {
                Debug.LogError(error);
            }
            return msg;
        }

        /// <summary>
        /// create MsgDecoder
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="msgType"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static MsgDecoder CreateInstance(Type msgType, out string error)
        {
            try
            {
                MethodInfo createBuilderMi = msgType.GetMethod("CreateBuilder", new Type[] { });
                if (createBuilderMi == null)
                {
                    error = "CreateBuilder method is not found";
                    return null;
                }

                object builder = createBuilderMi.Invoke(null, null);
                if (builder == null)
                {
                    error = "builder == null";
                    return null;
                }

                MethodInfo parseFromMi = msgType.GetMethod("ParseFrom", new[] { typeof(Stream) });
                if (parseFromMi == null)
                {
                    error = "ParseFrom method is not found";
                    return null;
                }

                MsgDecoder decoder = new MsgDecoder();
                decoder.mParseFromMi = parseFromMi;
                decoder.BuilderType = builder.GetType();
                error = null;
                return decoder;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return null;
            }

        }

        /// <summary>
        /// create MsgDecoder
        /// if error, use Debug.LogError
        /// </summary>
        /// <param name="msgType"></param>
        /// <returns></returns>
        public static MsgDecoder CreateInstance(Type msgType)
        {
            string error;
            MsgDecoder decoder = CreateInstance(msgType, out error);
            if (decoder == null)
            {
                Debug.LogError(error);
            }
            return decoder;
        }
    }
}
