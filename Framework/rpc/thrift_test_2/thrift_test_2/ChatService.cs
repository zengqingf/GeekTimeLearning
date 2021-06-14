/**
 * Autogenerated by Thrift Compiler (0.11.0)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace Common
{
  public partial class ChatService {
    public interface ISync {
      string Say(string thing);
      List<string> GetList(string function2_arg1, int function2_arg2, List<string> function2_arg3);
    }

    public interface Iface : ISync {
      #if SILVERLIGHT
      IAsyncResult Begin_Say(AsyncCallback callback, object state, string thing);
      string End_Say(IAsyncResult asyncResult);
      #endif
      #if SILVERLIGHT
      IAsyncResult Begin_GetList(AsyncCallback callback, object state, string function2_arg1, int function2_arg2, List<string> function2_arg3);
      List<string> End_GetList(IAsyncResult asyncResult);
      #endif
    }

    public class Client : IDisposable, Iface {
      public Client(TProtocol prot) : this(prot, prot)
      {
      }

      public Client(TProtocol iprot, TProtocol oprot)
      {
        iprot_ = iprot;
        oprot_ = oprot;
      }

      protected TProtocol iprot_;
      protected TProtocol oprot_;
      protected int seqid_;

      public TProtocol InputProtocol
      {
        get { return iprot_; }
      }
      public TProtocol OutputProtocol
      {
        get { return oprot_; }
      }


      #region " IDisposable Support "
      private bool _IsDisposed;

      // IDisposable
      public void Dispose()
      {
        Dispose(true);
      }
      

      protected virtual void Dispose(bool disposing)
      {
        if (!_IsDisposed)
        {
          if (disposing)
          {
            if (iprot_ != null)
            {
              ((IDisposable)iprot_).Dispose();
            }
            if (oprot_ != null)
            {
              ((IDisposable)oprot_).Dispose();
            }
          }
        }
        _IsDisposed = true;
      }
      #endregion


      
      #if SILVERLIGHT
      public IAsyncResult Begin_Say(AsyncCallback callback, object state, string thing)
      {
        return send_Say(callback, state, thing);
      }

      public string End_Say(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        return recv_Say();
      }

      #endif

      public string Say(string thing)
      {
        #if !SILVERLIGHT
        send_Say(thing);
        return recv_Say();

        #else
        var asyncResult = Begin_Say(null, null, thing);
        return End_Say(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_Say(AsyncCallback callback, object state, string thing)
      #else
      public void send_Say(string thing)
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("Say", TMessageType.Call, seqid_));
        Say_args args = new Say_args();
        args.Thing = thing;
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public string recv_Say()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        Say_result result = new Say_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        if (result.__isset.success) {
          return result.Success;
        }
        throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "Say failed: unknown result");
      }

      
      #if SILVERLIGHT
      public IAsyncResult Begin_GetList(AsyncCallback callback, object state, string function2_arg1, int function2_arg2, List<string> function2_arg3)
      {
        return send_GetList(callback, state, function2_arg1, function2_arg2, function2_arg3);
      }

      public List<string> End_GetList(IAsyncResult asyncResult)
      {
        oprot_.Transport.EndFlush(asyncResult);
        return recv_GetList();
      }

      #endif

      public List<string> GetList(string function2_arg1, int function2_arg2, List<string> function2_arg3)
      {
        #if !SILVERLIGHT
        send_GetList(function2_arg1, function2_arg2, function2_arg3);
        return recv_GetList();

        #else
        var asyncResult = Begin_GetList(null, null, function2_arg1, function2_arg2, function2_arg3);
        return End_GetList(asyncResult);

        #endif
      }
      #if SILVERLIGHT
      public IAsyncResult send_GetList(AsyncCallback callback, object state, string function2_arg1, int function2_arg2, List<string> function2_arg3)
      #else
      public void send_GetList(string function2_arg1, int function2_arg2, List<string> function2_arg3)
      #endif
      {
        oprot_.WriteMessageBegin(new TMessage("GetList", TMessageType.Call, seqid_));
        GetList_args args = new GetList_args();
        args.Function2_arg1 = function2_arg1;
        args.Function2_arg2 = function2_arg2;
        args.Function2_arg3 = function2_arg3;
        args.Write(oprot_);
        oprot_.WriteMessageEnd();
        #if SILVERLIGHT
        return oprot_.Transport.BeginFlush(callback, state);
        #else
        oprot_.Transport.Flush();
        #endif
      }

      public List<string> recv_GetList()
      {
        TMessage msg = iprot_.ReadMessageBegin();
        if (msg.Type == TMessageType.Exception) {
          TApplicationException x = TApplicationException.Read(iprot_);
          iprot_.ReadMessageEnd();
          throw x;
        }
        GetList_result result = new GetList_result();
        result.Read(iprot_);
        iprot_.ReadMessageEnd();
        if (result.__isset.success) {
          return result.Success;
        }
        throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "GetList failed: unknown result");
      }

    }
    public class Processor : TProcessor {
      public Processor(ISync iface)
      {
        iface_ = iface;
        processMap_["Say"] = Say_Process;
        processMap_["GetList"] = GetList_Process;
      }

      protected delegate void ProcessFunction(int seqid, TProtocol iprot, TProtocol oprot);
      private ISync iface_;
      protected Dictionary<string, ProcessFunction> processMap_ = new Dictionary<string, ProcessFunction>();

      public bool Process(TProtocol iprot, TProtocol oprot)
      {
        try
        {
          TMessage msg = iprot.ReadMessageBegin();
          ProcessFunction fn;
          processMap_.TryGetValue(msg.Name, out fn);
          if (fn == null) {
            TProtocolUtil.Skip(iprot, TType.Struct);
            iprot.ReadMessageEnd();
            TApplicationException x = new TApplicationException (TApplicationException.ExceptionType.UnknownMethod, "Invalid method name: '" + msg.Name + "'");
            oprot.WriteMessageBegin(new TMessage(msg.Name, TMessageType.Exception, msg.SeqID));
            x.Write(oprot);
            oprot.WriteMessageEnd();
            oprot.Transport.Flush();
            return true;
          }
          fn(msg.SeqID, iprot, oprot);
        }
        catch (IOException)
        {
          return false;
        }
        return true;
      }

      public void Say_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        Say_args args = new Say_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        Say_result result = new Say_result();
        try
        {
          result.Success = iface_.Say(args.Thing);
          oprot.WriteMessageBegin(new TMessage("Say", TMessageType.Reply, seqid)); 
          result.Write(oprot);
        }
        catch (TTransportException)
        {
          throw;
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine("Error occurred in processor:");
          Console.Error.WriteLine(ex.ToString());
          TApplicationException x = new TApplicationException        (TApplicationException.ExceptionType.InternalError," Internal error.");
          oprot.WriteMessageBegin(new TMessage("Say", TMessageType.Exception, seqid));
          x.Write(oprot);
        }
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

      public void GetList_Process(int seqid, TProtocol iprot, TProtocol oprot)
      {
        GetList_args args = new GetList_args();
        args.Read(iprot);
        iprot.ReadMessageEnd();
        GetList_result result = new GetList_result();
        try
        {
          result.Success = iface_.GetList(args.Function2_arg1, args.Function2_arg2, args.Function2_arg3);
          oprot.WriteMessageBegin(new TMessage("GetList", TMessageType.Reply, seqid)); 
          result.Write(oprot);
        }
        catch (TTransportException)
        {
          throw;
        }
        catch (Exception ex)
        {
          Console.Error.WriteLine("Error occurred in processor:");
          Console.Error.WriteLine(ex.ToString());
          TApplicationException x = new TApplicationException        (TApplicationException.ExceptionType.InternalError," Internal error.");
          oprot.WriteMessageBegin(new TMessage("GetList", TMessageType.Exception, seqid));
          x.Write(oprot);
        }
        oprot.WriteMessageEnd();
        oprot.Transport.Flush();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class Say_args : TBase
    {
      private string _thing;

      public string Thing
      {
        get
        {
          return _thing;
        }
        set
        {
          __isset.thing = true;
          this._thing = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool thing;
      }

      public Say_args() {
      }

      public void Read (TProtocol iprot)
      {
        iprot.IncrementRecursionDepth();
        try
        {
          TField field;
          iprot.ReadStructBegin();
          while (true)
          {
            field = iprot.ReadFieldBegin();
            if (field.Type == TType.Stop) { 
              break;
            }
            switch (field.ID)
            {
              case 1:
                if (field.Type == TType.String) {
                  Thing = iprot.ReadString();
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              default: 
                TProtocolUtil.Skip(iprot, field.Type);
                break;
            }
            iprot.ReadFieldEnd();
          }
          iprot.ReadStructEnd();
        }
        finally
        {
          iprot.DecrementRecursionDepth();
        }
      }

      public void Write(TProtocol oprot) {
        oprot.IncrementRecursionDepth();
        try
        {
          TStruct struc = new TStruct("Say_args");
          oprot.WriteStructBegin(struc);
          TField field = new TField();
          if (Thing != null && __isset.thing) {
            field.Name = "thing";
            field.Type = TType.String;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(Thing);
            oprot.WriteFieldEnd();
          }
          oprot.WriteFieldStop();
          oprot.WriteStructEnd();
        }
        finally
        {
          oprot.DecrementRecursionDepth();
        }
      }

      public override string ToString() {
        StringBuilder __sb = new StringBuilder("Say_args(");
        bool __first = true;
        if (Thing != null && __isset.thing) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Thing: ");
          __sb.Append(Thing);
        }
        __sb.Append(")");
        return __sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class Say_result : TBase
    {
      private string _success;

      public string Success
      {
        get
        {
          return _success;
        }
        set
        {
          __isset.success = true;
          this._success = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool success;
      }

      public Say_result() {
      }

      public void Read (TProtocol iprot)
      {
        iprot.IncrementRecursionDepth();
        try
        {
          TField field;
          iprot.ReadStructBegin();
          while (true)
          {
            field = iprot.ReadFieldBegin();
            if (field.Type == TType.Stop) { 
              break;
            }
            switch (field.ID)
            {
              case 0:
                if (field.Type == TType.String) {
                  Success = iprot.ReadString();
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              default: 
                TProtocolUtil.Skip(iprot, field.Type);
                break;
            }
            iprot.ReadFieldEnd();
          }
          iprot.ReadStructEnd();
        }
        finally
        {
          iprot.DecrementRecursionDepth();
        }
      }

      public void Write(TProtocol oprot) {
        oprot.IncrementRecursionDepth();
        try
        {
          TStruct struc = new TStruct("Say_result");
          oprot.WriteStructBegin(struc);
          TField field = new TField();

          if (this.__isset.success) {
            if (Success != null) {
              field.Name = "Success";
              field.Type = TType.String;
              field.ID = 0;
              oprot.WriteFieldBegin(field);
              oprot.WriteString(Success);
              oprot.WriteFieldEnd();
            }
          }
          oprot.WriteFieldStop();
          oprot.WriteStructEnd();
        }
        finally
        {
          oprot.DecrementRecursionDepth();
        }
      }

      public override string ToString() {
        StringBuilder __sb = new StringBuilder("Say_result(");
        bool __first = true;
        if (Success != null && __isset.success) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Success: ");
          __sb.Append(Success);
        }
        __sb.Append(")");
        return __sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class GetList_args : TBase
    {
      private string _function2_arg1;
      private int _function2_arg2;
      private List<string> _function2_arg3;

      public string Function2_arg1
      {
        get
        {
          return _function2_arg1;
        }
        set
        {
          __isset.function2_arg1 = true;
          this._function2_arg1 = value;
        }
      }

      public int Function2_arg2
      {
        get
        {
          return _function2_arg2;
        }
        set
        {
          __isset.function2_arg2 = true;
          this._function2_arg2 = value;
        }
      }

      public List<string> Function2_arg3
      {
        get
        {
          return _function2_arg3;
        }
        set
        {
          __isset.function2_arg3 = true;
          this._function2_arg3 = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool function2_arg1;
        public bool function2_arg2;
        public bool function2_arg3;
      }

      public GetList_args() {
      }

      public void Read (TProtocol iprot)
      {
        iprot.IncrementRecursionDepth();
        try
        {
          TField field;
          iprot.ReadStructBegin();
          while (true)
          {
            field = iprot.ReadFieldBegin();
            if (field.Type == TType.Stop) { 
              break;
            }
            switch (field.ID)
            {
              case 1:
                if (field.Type == TType.String) {
                  Function2_arg1 = iprot.ReadString();
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              case 2:
                if (field.Type == TType.I32) {
                  Function2_arg2 = iprot.ReadI32();
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              case 3:
                if (field.Type == TType.List) {
                  {
                    Function2_arg3 = new List<string>();
                    TList _list0 = iprot.ReadListBegin();
                    for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                    {
                      string _elem2;
                      _elem2 = iprot.ReadString();
                      Function2_arg3.Add(_elem2);
                    }
                    iprot.ReadListEnd();
                  }
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              default: 
                TProtocolUtil.Skip(iprot, field.Type);
                break;
            }
            iprot.ReadFieldEnd();
          }
          iprot.ReadStructEnd();
        }
        finally
        {
          iprot.DecrementRecursionDepth();
        }
      }

      public void Write(TProtocol oprot) {
        oprot.IncrementRecursionDepth();
        try
        {
          TStruct struc = new TStruct("GetList_args");
          oprot.WriteStructBegin(struc);
          TField field = new TField();
          if (Function2_arg1 != null && __isset.function2_arg1) {
            field.Name = "function2_arg1";
            field.Type = TType.String;
            field.ID = 1;
            oprot.WriteFieldBegin(field);
            oprot.WriteString(Function2_arg1);
            oprot.WriteFieldEnd();
          }
          if (__isset.function2_arg2) {
            field.Name = "function2_arg2";
            field.Type = TType.I32;
            field.ID = 2;
            oprot.WriteFieldBegin(field);
            oprot.WriteI32(Function2_arg2);
            oprot.WriteFieldEnd();
          }
          if (Function2_arg3 != null && __isset.function2_arg3) {
            field.Name = "function2_arg3";
            field.Type = TType.List;
            field.ID = 3;
            oprot.WriteFieldBegin(field);
            {
              oprot.WriteListBegin(new TList(TType.String, Function2_arg3.Count));
              foreach (string _iter3 in Function2_arg3)
              {
                oprot.WriteString(_iter3);
              }
              oprot.WriteListEnd();
            }
            oprot.WriteFieldEnd();
          }
          oprot.WriteFieldStop();
          oprot.WriteStructEnd();
        }
        finally
        {
          oprot.DecrementRecursionDepth();
        }
      }

      public override string ToString() {
        StringBuilder __sb = new StringBuilder("GetList_args(");
        bool __first = true;
        if (Function2_arg1 != null && __isset.function2_arg1) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Function2_arg1: ");
          __sb.Append(Function2_arg1);
        }
        if (__isset.function2_arg2) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Function2_arg2: ");
          __sb.Append(Function2_arg2);
        }
        if (Function2_arg3 != null && __isset.function2_arg3) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Function2_arg3: ");
          __sb.Append(Function2_arg3);
        }
        __sb.Append(")");
        return __sb.ToString();
      }

    }


    #if !SILVERLIGHT
    [Serializable]
    #endif
    public partial class GetList_result : TBase
    {
      private List<string> _success;

      public List<string> Success
      {
        get
        {
          return _success;
        }
        set
        {
          __isset.success = true;
          this._success = value;
        }
      }


      public Isset __isset;
      #if !SILVERLIGHT
      [Serializable]
      #endif
      public struct Isset {
        public bool success;
      }

      public GetList_result() {
      }

      public void Read (TProtocol iprot)
      {
        iprot.IncrementRecursionDepth();
        try
        {
          TField field;
          iprot.ReadStructBegin();
          while (true)
          {
            field = iprot.ReadFieldBegin();
            if (field.Type == TType.Stop) { 
              break;
            }
            switch (field.ID)
            {
              case 0:
                if (field.Type == TType.List) {
                  {
                    Success = new List<string>();
                    TList _list4 = iprot.ReadListBegin();
                    for( int _i5 = 0; _i5 < _list4.Count; ++_i5)
                    {
                      string _elem6;
                      _elem6 = iprot.ReadString();
                      Success.Add(_elem6);
                    }
                    iprot.ReadListEnd();
                  }
                } else { 
                  TProtocolUtil.Skip(iprot, field.Type);
                }
                break;
              default: 
                TProtocolUtil.Skip(iprot, field.Type);
                break;
            }
            iprot.ReadFieldEnd();
          }
          iprot.ReadStructEnd();
        }
        finally
        {
          iprot.DecrementRecursionDepth();
        }
      }

      public void Write(TProtocol oprot) {
        oprot.IncrementRecursionDepth();
        try
        {
          TStruct struc = new TStruct("GetList_result");
          oprot.WriteStructBegin(struc);
          TField field = new TField();

          if (this.__isset.success) {
            if (Success != null) {
              field.Name = "Success";
              field.Type = TType.List;
              field.ID = 0;
              oprot.WriteFieldBegin(field);
              {
                oprot.WriteListBegin(new TList(TType.String, Success.Count));
                foreach (string _iter7 in Success)
                {
                  oprot.WriteString(_iter7);
                }
                oprot.WriteListEnd();
              }
              oprot.WriteFieldEnd();
            }
          }
          oprot.WriteFieldStop();
          oprot.WriteStructEnd();
        }
        finally
        {
          oprot.DecrementRecursionDepth();
        }
      }

      public override string ToString() {
        StringBuilder __sb = new StringBuilder("GetList_result(");
        bool __first = true;
        if (Success != null && __isset.success) {
          if(!__first) { __sb.Append(", "); }
          __first = false;
          __sb.Append("Success: ");
          __sb.Append(Success);
        }
        __sb.Append(")");
        return __sb.ToString();
      }

    }

  }
}
