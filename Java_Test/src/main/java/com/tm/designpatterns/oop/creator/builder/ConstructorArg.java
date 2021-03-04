package com.tm.designpatterns.oop.creator.builder;

public class ConstructorArg {
    private boolean isRef;
    private Class<?> type;
    private Object arg;

    public boolean isRef() { return isRef; }
    public Class<?> getType() { return type; }
    public Object getArg() { return arg; }

    private ConstructorArg(Builder builder) {
        if(null != builder) {
            setRef(builder.isRef)
                .setArg(builder.arg)
                    .setType(builder.type);
        }
    }

    public ConstructorArg setRef(boolean isRef) {
        this.isRef = isRef;
        return this;
    }

    public ConstructorArg setType(Class<?> type) {
        this.type = type;
        return this;
    }

    public ConstructorArg setArg(Object obj) {
        this.arg = obj;
        return this;
    }

    public static class Builder {
        private boolean isRef;
        private Class type;
        private Object arg;

        public static Builder create() { return new Builder(); }

        public ConstructorArg build() {
            if(isRef) {
                if(arg == null || !(arg instanceof String)) {
                    throw new IllegalArgumentException("...");
                }
                if(type != null) {
                    throw new IllegalArgumentException("...");
                }
            }else {
                if(arg == null) {
                    throw new IllegalArgumentException("...");
                }
                if(type == null) {
                    throw new IllegalArgumentException("...");
                }
            }
            return new ConstructorArg(this);
        }

        public Builder setRef(boolean isRef) {
            this.isRef = isRef;
            return this;
        }

        public Builder setType(Class<?> type) {
            this.type = type;
            return this;
        }

        public Builder setArg(Object obj) {
            this.arg = obj;
            return this;
        }
    }
}
