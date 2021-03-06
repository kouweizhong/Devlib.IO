﻿//-----------------------------------------------------------------------
// <copyright file="IocRegistrationBuilder.cs" company="YuGuan Corporation">
//     Copyright (c) YuGuan Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace DevLib.Ioc
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// IoC Registration builder.
    /// </summary>
    public class IocRegistrationBuilder : IDisposable
    {
        /// <summary>
        /// Field _registrationType.
        /// </summary>
        private readonly Type _registrationType;

        /// <summary>
        /// Field _registrationName.
        /// </summary>
        private readonly string _registrationName;

        /// <summary>
        /// Field _instanceType.
        /// </summary>
        private readonly Type _instanceType;

        /// <summary>
        /// Field _instanceTypeArguments.
        /// </summary>
        private readonly Type[] _instanceTypeArguments;

        /// <summary>
        /// Field _cachedRegistrationValue.
        /// </summary>
        private object _cachedRegistrationValue;

        /// <summary>
        /// Field _registrationBuilder.
        /// </summary>
        private Converter<IocContainer, object> _registrationBuilder;

        /// <summary>
        /// Field _disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder" /> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="instanceTypeArguments">The instance type arguments.</param>
        /// <param name="name">The name of the registration.</param>
        public IocRegistrationBuilder(Type registrationType, Type instanceType, Type[] instanceTypeArguments, string name)
        {
            this._registrationType = registrationType;
            this._registrationName = name;
            this._instanceType = instanceType;
            this._instanceTypeArguments = instanceTypeArguments;
            this.IsNamed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder" /> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="instanceType">Type of the instance.</param>
        /// <param name="instanceTypeArguments">The instance type arguments.</param>
        public IocRegistrationBuilder(Type registrationType, Type instanceType, Type[] instanceTypeArguments)
        {
            this._registrationType = registrationType;
            this._registrationName = Guid.NewGuid().ToString();
            this._instanceType = instanceType;
            this._instanceTypeArguments = instanceTypeArguments;
            this.IsNamed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="registrationBuilder">The registration builder.</param>
        /// <param name="name">The name of the registration.</param>
        public IocRegistrationBuilder(Type registrationType, Converter<IocContainer, object> registrationBuilder, string name)
        {
            this._registrationType = registrationType;
            this._registrationName = name;
            this._registrationBuilder = registrationBuilder;
            this.IsNamed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="registrationBuilder">The registration builder.</param>
        public IocRegistrationBuilder(Type registrationType, Converter<IocContainer, object> registrationBuilder)
        {
            this._registrationType = registrationType;
            this._registrationName = Guid.NewGuid().ToString();
            this._registrationBuilder = registrationBuilder;
            this.IsNamed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="registrationValue">The registration value.</param>
        /// <param name="name">The name of the registration.</param>
        public IocRegistrationBuilder(Type registrationType, object registrationValue, string name)
        {
            this._registrationType = registrationType;
            this._registrationName = name;
            this._cachedRegistrationValue = registrationValue;
            this.IsNamed = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IocRegistrationBuilder"/> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="registrationValue">The registration value.</param>
        public IocRegistrationBuilder(Type registrationType, object registrationValue)
        {
            this._registrationType = registrationType;
            this._registrationName = Guid.NewGuid().ToString();
            this._cachedRegistrationValue = registrationValue;
            this.IsNamed = false;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="IocRegistrationBuilder" /> class.
        /// </summary>
        ~IocRegistrationBuilder()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this registration has a cached instance.
        /// </summary>
        public bool HasInstance
        {
            get
            {
                return this._cachedRegistrationValue != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this registration builder delegate is evaluated.
        /// </summary>
        public bool IsEvaluated
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is named registration.
        /// </summary>
        public bool IsNamed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the registration value.
        /// </summary>
        /// <param name="container">The container to use.</param>
        /// <param name="createNew">true to create new instance if possible every time; otherwise, get the same instance every time.</param>
        /// <returns>The registration value.</returns>
        public object GetValue(IocContainer container, bool createNew = false)
        {
            this.CheckDisposed();

            if (createNew)
            {
                var result = this.CreateService(container);

                if (result == null)
                {
                    return this._cachedRegistrationValue;
                }

                if (this._cachedRegistrationValue == null)
                {
                    this._cachedRegistrationValue = result;
                }

                return result;
            }
            else
            {
                return this._cachedRegistrationValue ?? (this._cachedRegistrationValue = this.CreateService(container));
            }
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="IocRegistrationBuilder" /> class.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="IocRegistrationBuilder" /> class.
        /// protected virtual for non-sealed class; private for sealed class.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

            if (disposing)
            {
                // dispose managed resources
                ////if (managedResource != null)
                ////{
                ////    managedResource.Dispose();
                ////    managedResource = null;
                ////}

                this._registrationBuilder = null;

                IDisposable disposable = this._cachedRegistrationValue as IDisposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                    this._cachedRegistrationValue = null;
                }
            }

            // free native resources
            ////if (nativeResource != IntPtr.Zero)
            ////{
            ////    Marshal.FreeHGlobal(nativeResource);
            ////    nativeResource = IntPtr.Zero;
            ////}
        }

        /// <summary>
        /// Creates the service.
        /// </summary>
        /// <param name="container">The container to use.</param>
        /// <returns>The registration value.</returns>
        private object CreateService(IocContainer container)
        {
            this.IsEvaluated = true;

            if (this._registrationBuilder != null)
            {
                return this._registrationBuilder(container);
            }
            else if (this._instanceType != null)
            {
                return this.CreateInstance(this._instanceType, this._instanceTypeArguments);
            }

            return null;
        }

        /// <summary>
        /// Creates an instance of the specified type using the constructor that best matches the specified parameters.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="typeArguments">An array of types to be substituted for the type parameters of the current generic method definition.</param>
        /// <returns>A reference to the newly created object.</returns>
        [SecurityPermission(SecurityAction.Demand, Unrestricted = true)]
        private object CreateInstance(Type type, Type[] typeArguments)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }

            object result = null;

            if (typeArguments != null && typeArguments.Length > 0)
            {
                type = type.MakeGenericType(typeArguments);
            }

            try
            {
                result = Activator.CreateInstance(type);
            }
            catch
            {
                try
                {
                    result = FormatterServices.GetUninitializedObject(type);
                }
                catch
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Method CheckDisposed.
        /// </summary>
        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException("DevLib.Ioc.IocRegistrationBuilder");
            }
        }
    }
}
